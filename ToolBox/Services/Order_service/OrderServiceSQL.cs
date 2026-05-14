using Microsoft.Data.SqlClient;
using Praca_Inżynierska_v1.MVVM.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Praca_Inżynierska_v1.Services.Order_Service
{
    public class OrderServiceSQL : IOrderService
    {
        private readonly string _connectionString;

        public OrderServiceSQL(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            var orders = new List<Order>();

            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            string query = @"
                SELECT z.ID_Zamowienia, z.ID_Klienta, z.ID_Uzytkownika, z.Data_zlozenia, z.Status, z.Uwagi,
                       z.NumerZamowienia, k.Imie, k.Nazwisko, u.Login
                FROM Zamowienia z
                JOIN Klienci k ON z.ID_Klienta = k.ID_Klienta
                JOIN Uzytkownicy u ON z.ID_Uzytkownika = u.ID_Uzytkownika";

            using var cmd = new SqlCommand(query, conn);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                orders.Add(new Order
                {
                    OrderId = reader.GetInt32(0),
                    ClientId = reader.GetInt32(1),
                    UserId = reader.GetInt32(2),
                    OrderDate = reader.GetDateTime(3),
                    Status = reader.GetString(4),
                    Notes = reader.IsDBNull(5) ? null : reader.GetString(5),
                    OrderNumber = reader.GetString(6),
                    ClientName = $"{reader.GetString(7)} {reader.GetString(8)}",
                    UserName = reader.GetString(9)
                });
            }

            return orders;
        }

        public async Task<List<OrderItem>> GetOrderItemsAsync(int orderId)
        {
            var items = new List<OrderItem>();

            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            string query = @"
                SELECT zp.ID_ZamowienieProdukt, zp.ID_Zamowienia, zp.ID_Produktu, zp.Ilosc,
                       zp.CenaJednostkowa, zp.Rabat, p.Nazwa
                FROM ZamowienieProdukt zp
                JOIN Produkty p ON zp.ID_Produktu = p.ID_Produktu
                WHERE zp.ID_Zamowienia = @OrderId";

            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@OrderId", orderId);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                items.Add(new OrderItem
                {
                    OrderItemId = reader.GetInt32(0),
                    OrderId = reader.GetInt32(1),
                    ProductId = reader.GetInt32(2),
                    Quantity = reader.GetDecimal(3),
                    UnitPrice = reader.GetDecimal(4),
                    Discount = reader.IsDBNull(5) ? null : reader.GetDecimal(5),
                    ProductName = reader.GetString(6)
                });
            }

            return items;
        }

        public async Task AddOrderAsync(Order order, List<OrderItem> items)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            var transaction = conn.BeginTransaction();

            try
            {
                // Wygeneruj numer zamówienia
                var orderNumber = $"ORD-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString()[..4].ToUpper()}";
                order.OrderNumber = orderNumber;

                // Dodaj zamówienie przez procedurę
                using var cmdAddOrder = new SqlCommand("DodajZamowienie", conn, transaction);
                cmdAddOrder.CommandType = System.Data.CommandType.StoredProcedure;

                cmdAddOrder.Parameters.AddWithValue("@ID_Klienta", order.ClientId);
                cmdAddOrder.Parameters.AddWithValue("@ID_Uzytkownika", order.UserId);
                cmdAddOrder.Parameters.AddWithValue("@Data_zlozenia", order.OrderDate);
                cmdAddOrder.Parameters.AddWithValue("@Status", order.Status);
                cmdAddOrder.Parameters.AddWithValue("@Uwagi", (object?)order.Notes ?? DBNull.Value);
                cmdAddOrder.Parameters.AddWithValue("@NumerZamowienia", order.OrderNumber);

                var newOrderId = Convert.ToInt32(await cmdAddOrder.ExecuteScalarAsync());

                // Dodaj produkty do zamówienia przez procedurę
                foreach (var item in items)
                {
                    using var cmdAddItem = new SqlCommand("DodajPozycjeZamowienia", conn, transaction);
                    cmdAddItem.CommandType = System.Data.CommandType.StoredProcedure;

                    cmdAddItem.Parameters.AddWithValue("@ID_Zamowienia", newOrderId);
                    cmdAddItem.Parameters.AddWithValue("@ID_Produktu", item.ProductId);
                    cmdAddItem.Parameters.AddWithValue("@Ilosc", item.Quantity);
                    cmdAddItem.Parameters.AddWithValue("@CenaJednostkowa", item.UnitPrice);
                    cmdAddItem.Parameters.AddWithValue("@Rabat", (object?)item.Discount ?? DBNull.Value);

                    await cmdAddItem.ExecuteNonQueryAsync();
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task UpdateOrderAsync(Order order)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new SqlCommand("AktualizujZamowienie", conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@ID_Zamowienia", order.OrderId);
            cmd.Parameters.AddWithValue("@ID_Klienta", order.ClientId);
            cmd.Parameters.AddWithValue("@ID_Uzytkownika", order.UserId);
            cmd.Parameters.AddWithValue("@Data_zlozenia", order.OrderDate);
            cmd.Parameters.AddWithValue("@Status", order.Status);
            cmd.Parameters.AddWithValue("@Uwagi", (object?)order.Notes ?? DBNull.Value);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task DeleteOrderAsync(int orderId)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new SqlCommand("UsunZamowienie", conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ID_Zamowienia", orderId);

            await cmd.ExecuteNonQueryAsync();
        }
    }
}