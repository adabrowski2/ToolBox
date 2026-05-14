using System.Data;
using Microsoft.Data.SqlClient;
using Praca_Inżynierska_v1.MVVM.Model;

namespace Praca_Inżynierska_v1.Services.Delivery_Service
{
    public class DeliveryServiceSQL
    {
        private readonly string _connectionString;

        public DeliveryServiceSQL(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Delivery> GetAllDeliveries()
        {
            var deliveries = new List<Delivery>();

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(@"
                SELECT 
                    D.ID_Dostawy,
                    D.ID_Uzytkownika,
                    D.Data_zamowienia,
                    D.Data_przyjecia,
                    D.Status,
                    D.Komentarz,
                    COUNT(PD.ID_Pozycji_dostawy) AS ItemCount
                FROM Dostawy D
                LEFT JOIN Pozycja_dostawy PD ON D.ID_Dostawy = PD.ID_Dostawy
                GROUP BY D.ID_Dostawy, D.ID_Uzytkownika, D.Data_zamowienia, D.Data_przyjecia, D.Status, D.Komentarz
                ORDER BY D.Data_zamowienia DESC", conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        deliveries.Add(new Delivery
                        {
                            DeliveryId = reader.GetInt32(0),
                            UserId = reader.GetInt32(1),
                            OrderDate = reader.GetDateTime(2),
                            ReceivedDate = reader.IsDBNull(3) ? (DateTime?)null : reader.GetDateTime(3),
                            Status = reader.GetString(4),
                            Comment = reader.IsDBNull(5) ? null : reader.GetString(5),
                            ItemCount = reader.GetInt32(6)
                        });
                    }
                }
            }

            return deliveries;
        }

        public List<DeliveryItem> GetDeliveryItems(int deliveryId)
        {
            var items = new List<DeliveryItem>();

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(@"
        SELECT 
            PD.ID_Pozycji_dostawy,
            PD.ID_Dostawy,
            PD.ID_Produktu,
            P.Nazwa,
            PD.Ilosc,
            PD.Ilosc_przyjeta
        FROM Pozycja_dostawy PD
        JOIN Produkty P ON P.ID_Produktu = PD.ID_Produktu
        WHERE PD.ID_Dostawy = @did", conn))
            {
                cmd.Parameters.AddWithValue("@did", deliveryId);
                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new DeliveryItem
                        {
                            DeliveryItemId = reader.GetInt32(0),
                            DeliveryID = reader.GetInt32(1),
                            ProductID = reader.GetInt32(2),
                            Name = reader.GetString(3),
                            Quantity = reader.GetInt32(4), // Ilosc
                            ReceivedQuantity = reader.IsDBNull(5) ? 0 : reader.GetInt32(5) // Ilosc_przyjeta
                        });
                    }
                }
            }

            return items;
        }

        public void MarkDeliveryAsReceived(int deliveryId)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(@"
        UPDATE Dostawy 
        SET Status = 'Zrealizowana', Data_przyjecia = GETDATE()
        WHERE ID_Dostawy = @DeliveryId", conn))
            {
                cmd.Parameters.AddWithValue("@DeliveryId", deliveryId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void MarkDeliveryAsReceivedWithQuantities(int deliveryId, List<DeliveryItem> items)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var transaction = conn.BeginTransaction();

                try
                {
                    foreach (var item in items)
                    {
                        var cmd = new SqlCommand(@"
                    UPDATE Pozycja_dostawy
                    SET Ilosc_przyjeta = @received
                    WHERE ID_Pozycji_dostawy = @id", conn, transaction);

                        cmd.Parameters.AddWithValue("@received", item.ReceivedQuantity);
                        cmd.Parameters.AddWithValue("@id", item.DeliveryItemId);
                        cmd.ExecuteNonQuery();

                        // Aktualizacja stanu magazynowego produktu:
                        var stockCmd = new SqlCommand(@"
                    UPDATE Produkty
                    SET Ilosc = Ilosc + @received
                    WHERE ID_Produktu = @pid", conn, transaction);

                        stockCmd.Parameters.AddWithValue("@received", item.ReceivedQuantity);
                        stockCmd.Parameters.AddWithValue("@pid", item.ProductID);
                        stockCmd.ExecuteNonQuery();
                    }

                    var updateDelivery = new SqlCommand(@"
                UPDATE Dostawy
                SET Status = 'Zrealizowana', Data_przyjecia = GETDATE()
                WHERE ID_Dostawy = @did", conn, transaction);

                    updateDelivery.Parameters.AddWithValue("@did", deliveryId);
                    updateDelivery.ExecuteNonQuery();

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

    }
}