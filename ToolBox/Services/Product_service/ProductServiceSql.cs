using Microsoft.Data.SqlClient;
using Praca_Inżynierska_v1.MVVM.Model;
using Praca_Inżynierska_v1.Services.Product_service;

namespace Praca_Inżynierska_v1.Services.Product_Service
{
    public class ProductServiceSql : IProductService
    {
        private readonly string _connectionString;



        public ProductServiceSql(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            var products = new List<Product>();

            string query = @"
            SELECT 
                P.ID_Produktu,
                P.Nazwa,
                P.Cena,
                P.Ilosc,
                P.Jednostka_miary,
                P.Próg_minimalny,
                P.Aktywny,
                R.Wartosc AS Rabat,
                R.DataOd,
                R.DataDo
            FROM Produkty P
            LEFT JOIN Rabaty R ON P.ID_Produktu = R.ID_Produktu AND R.DataDo >= GETDATE()";

            try
            {
                using (var sqlCon = new SqlConnection(_connectionString))
                {
                    await sqlCon.OpenAsync();

                    using (var cmd = new SqlCommand(query, sqlCon))
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            decimal basePrice = reader.GetDecimal(2);
                            decimal? discount = reader.IsDBNull(7) ? null : reader.GetDecimal(7);
/*                          decimal finalPrice = discount.HasValue ? basePrice * (1 - discount.Value / 100m) : basePrice;*/

                            var product = new Product
                            {
                                ProductId = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Price = Math.Round(basePrice, 2), // Cena po rabacie
                                Quantity = reader.GetDecimal(3),
                                Unit = reader.GetString(4),
                                MinimumStock = !reader.IsDBNull(5) ? reader.GetInt32(5) : (int?)null,
                                IsActive = !reader.IsDBNull(6) ? reader.GetBoolean(6) : (bool?)null,
                                Discount = discount, // dodatkowe pole w modelu Product
                                DiscountStartDate = reader.IsDBNull(8) ? null : reader.GetDateTime(8),
                                DiscountEndDate = reader.IsDBNull(9) ? null : reader.GetDateTime(9)

                            };
                            products.Add(product);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Błąd podczas wczytywania produktów: " + ex.Message, ex);
            }

            return products;
        }

        public async Task UpdateProductsAsync(IEnumerable<Product> products)
        {
            string updateQuery = @"
                UPDATE Produkty SET Ilosc = @Ilosc, Aktywny = @Aktywny WHERE ID_Produktu = @ID_Produktu";

            try
            {
                using (var sqlCon = new SqlConnection(_connectionString))
                {
                    await sqlCon.OpenAsync();

                    foreach (var product in products)
                    {
                        using (var cmd = new SqlCommand(updateQuery, sqlCon))
                        {
                            cmd.Parameters.AddWithValue("@ID_Produktu", product.ProductId);
                            cmd.Parameters.AddWithValue("@Ilosc", product.Quantity);
                            cmd.Parameters.AddWithValue("@Aktywny", product.IsActive.HasValue ? (object)product.IsActive.Value : DBNull.Value);

                            await cmd.ExecuteNonQueryAsync();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Błąd podczas zapisywania produktów: " + ex.Message, ex);
            }
        }

        public async Task DecreaseQuantityAsync(int productId, decimal quantity)
        {
            string query = @"UPDATE Produkty SET Ilosc = Ilosc - @Ilosc WHERE ID_Produktu = @ID_Produktu";
            try
            {
                using (var sqlCon = new SqlConnection(_connectionString))
                {
                    await sqlCon.OpenAsync();

                    using (var cmd = new SqlCommand(query, sqlCon))
                    {
                        cmd.Parameters.AddWithValue("@ID_Produktu", productId);
                        cmd.Parameters.AddWithValue("@Ilosc", quantity);

                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Błąd podczas aktualizacji stanu magazynowego: " + ex.Message, ex);
            }
        }

        public async Task UpdateProductQuantityAsync(int productId, decimal quantity)
        {
            string query = "UPDATE Produkty SET Ilosc = @Ilosc WHERE ID_Produktu = @ID_Produktu";

            try
            {
                using (var sqlCon = new SqlConnection(_connectionString))
                {
                    await sqlCon.OpenAsync();

                    using (var cmd = new SqlCommand(query, sqlCon))
                    {
                        cmd.Parameters.AddWithValue("@ID_Produktu", productId);
                        cmd.Parameters.AddWithValue("@Ilosc", quantity);

                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Błąd podczas aktualizacji ilości produktu: " + ex.Message, ex);
            }
        }
    }
}
