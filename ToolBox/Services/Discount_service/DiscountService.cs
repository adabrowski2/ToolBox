using Microsoft.Data.SqlClient;

namespace Praca_Inżynierska_v1.Services.Discount_Service
{
    public class DiscountService
    {
        private readonly string _connectionString;

        public DiscountService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void SaveDiscount(int productId, decimal discountValue, DateTime validTo)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            DateTime today = DateTime.Today;
            decimal currentPrice;

            // Pobiera AKTUALNĄ cenę z Produktów
            using (var cmd = new SqlCommand(@"
            SELECT TOP 1 Cena_Oryginalna 
            FROM Rabaty 
            WHERE ID_Produktu = @ID AND DataDo >= @Today
            ORDER BY DataDo DESC", conn))
            {
                cmd.Parameters.AddWithValue("@ID", productId);
                cmd.Parameters.AddWithValue("@Today", today);

                var result = cmd.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    currentPrice = (decimal)result;
                }
                else
                {
                    // Brak aktywnego rabatu – pobieramy z Produktów
                    using var fallbackCmd = new SqlCommand("SELECT Cena FROM Produkty WHERE ID_Produktu = @ID", conn);
                    fallbackCmd.Parameters.AddWithValue("@ID", productId);
                    currentPrice = (decimal)fallbackCmd.ExecuteScalar();
                }
            }

            // Oblicza przecenioną cenę na podstawie aktualnej ceny
            decimal discountedPrice = Math.Round(currentPrice * (1 - discountValue / 100m), 2);

            // Sprawda czy istnieje aktywny rabat
            bool hasActive;
            using (var checkCmd = new SqlCommand(@"
        SELECT COUNT(*) FROM Rabaty 
        WHERE ID_Produktu = @ID_Produktu AND DataDo >= @Today", conn))
            {
                checkCmd.Parameters.AddWithValue("@ID_Produktu", productId);
                checkCmd.Parameters.AddWithValue("@Today", today);
                hasActive = (int)checkCmd.ExecuteScalar() > 0;
            }

            // Jeśli brak aktywnego rabatu → zapisz oryginalną cenę
            if (!hasActive)
            {
                string insertQuery = @"
            INSERT INTO Rabaty (ID_Produktu, Wartosc, DataOd, DataDo, Cena_Oryginalna)
            VALUES (@ID_Produktu, @Wartosc, @DataOd, @DataDo, @Cena_Oryginalna)";

                using var insertCmd = new SqlCommand(insertQuery, conn);
                insertCmd.Parameters.AddWithValue("@ID_Produktu", productId);
                insertCmd.Parameters.AddWithValue("@Wartosc", discountValue);
                insertCmd.Parameters.AddWithValue("@DataOd", today);
                insertCmd.Parameters.AddWithValue("@DataDo", validTo);
                insertCmd.Parameters.AddWithValue("@Cena_Oryginalna", currentPrice);
                insertCmd.ExecuteNonQuery();
            }
            else
            {
                // Jeśli rabat istnieje → zaktualizuj tylko wartość i datę (bez ruszania oryginalnej ceny)
                string updateQuery = @"
            UPDATE Rabaty 
            SET Wartosc = @Wartosc, DataDo = @DataDo 
            WHERE ID_Produktu = @ID_Produktu AND DataDo >= @Today";

                using var updateCmd = new SqlCommand(updateQuery, conn);
                updateCmd.Parameters.AddWithValue("@ID_Produktu", productId);
                updateCmd.Parameters.AddWithValue("@Wartosc", discountValue);
                updateCmd.Parameters.AddWithValue("@DataDo", validTo);
                updateCmd.Parameters.AddWithValue("@Today", today);
                updateCmd.ExecuteNonQuery();
            }

            // zaktualizuj cenę w Produkty
            using var updateProductCmd = new SqlCommand("UPDATE Produkty SET Cena = @Cena WHERE ID_Produktu = @ID", conn);
            updateProductCmd.Parameters.AddWithValue("@Cena", discountedPrice);
            updateProductCmd.Parameters.AddWithValue("@ID", productId);
            updateProductCmd.ExecuteNonQuery();
        }

        public decimal? GetActiveDiscountForProduct(int productId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string query = @"
                    SELECT TOP 1 Wartosc 
                    FROM Rabaty 
                    WHERE ID_Produktu = @ID_Produktu AND DataDo >= GETDATE()
                    ORDER BY DataDo DESC";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ID_Produktu", productId);
                    var result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToDecimal(result) : (decimal?)null;
                }
            }
        }

        public void DiscountExpire()
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                // Przywraca oryginalną cenę
                string query = @"
                    UPDATE Produkty
                    SET Cena = R.Cena_Oryginalna
                    FROM Produkty P
                    INNER JOIN Rabaty R ON P.ID_Produktu = R.ID_Produktu
                    WHERE R.DataDo < GETDATE()
                    AND P.Cena <> R.Cena_Oryginalna";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                // Usuwa wygasłe rabaty
                string deleteQuery = "DELETE FROM Rabaty WHERE DataDo < GETDATE()";
                using (var deleteCmd = new SqlCommand(deleteQuery, conn))
                {
                    deleteCmd.ExecuteNonQuery();
                }
            }
        }
        public void DeactivateDiscount(int productId)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            DateTime now = DateTime.Now;

            // Przywrócenie oryginalnej ceny
            string updateProductQuery = @"
        UPDATE Produkty
        SET Cena = R.Cena_Oryginalna
        FROM Produkty P
        INNER JOIN Rabaty R ON P.ID_Produktu = R.ID_Produktu
        WHERE P.ID_Produktu = @ID
          AND R.DataDo >= @Now"; // tylko jeśli rabat nadal obowiązuje

            using (var updateCmd = new SqlCommand(updateProductQuery, conn))
            {
                updateCmd.Parameters.AddWithValue("@ID", productId);
                updateCmd.Parameters.AddWithValue("@Now", now);
                updateCmd.ExecuteNonQuery();
            }

            // Zakończenie rabatu – ustawiamy DataDo = teraz
            string expireDiscountQuery = @"
        UPDATE Rabaty
        SET DataDo = @Now
        WHERE ID_Produktu = @ID AND DataDo >= @Now";

            using (var expireCmd = new SqlCommand(expireDiscountQuery, conn))
            {
                expireCmd.Parameters.AddWithValue("@ID", productId);
                expireCmd.Parameters.AddWithValue("@Now", now);
                expireCmd.ExecuteNonQuery();
            }
        }
    }
}