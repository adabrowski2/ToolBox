using Microsoft.Data.SqlClient;
using Praca_Inżynierska_v1.MVVM.Model;

namespace Praca_Inżynierska_v1.Services.Client_Service
{
    public class ClientServiceSql : IClientService
    {
        private readonly string _connectionString;

        public ClientServiceSql(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<Client>> GetAllClientsAsync()
        {
            var clients = new List<Client>();
            string query = @"
                SELECT ID_Klienta, Typ_Klienta, Imie, Nazwisko, Nazwa_Firmy, NIP, REGON, Adres, Telefon, Email FROM Klienci";

            try
            {
                using (var sqlCon = new SqlConnection(_connectionString))
                {
                    await sqlCon.OpenAsync();

                    using (var cmd = new SqlCommand(query, sqlCon))
                    {
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var client = new Client
                                {
                                    ClientId = reader.GetInt32(0),
                                    ClientType = reader.GetString(1),
                                    FirstName = reader.GetString(2),
                                    LastName = reader.GetString(3),
                                    CompanyName = !reader.IsDBNull(4) ? reader.GetString(4) : null,
                                    NIP = !reader.IsDBNull(5) ? reader.GetString(5) : null,
                                    REGON = !reader.IsDBNull(6) ? reader.GetString(6) : null,
                                    Address = !reader.IsDBNull(7) ? reader.GetString(7) : null,
                                    Phone = !reader.IsDBNull(8) ? reader.GetString(8) : null,
                                    Email = !reader.IsDBNull(9) ? reader.GetString(9) : null
                                };
                                clients.Add(client);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Błąd podczas wczytywania klientów: " + ex.Message, ex);
            }

            return clients;
        }

        public async Task UpdateClientsAsync(IEnumerable<Client> clients)
        {
            string updateQuery = @"UPDATE Klienci SET Typ_Klienta = @Typ_Klienta, Imie = @Imie, Nazwisko = @Nazwisko, Nazwa_Firmy = @Nazwa_Firmy, NIP = @NIP, REGON = @REGON, Adres = @Adres, Telefon = @Telefon, Email = @Email 
                                 WHERE ID_Klienta = @ID_Klienta";

            try
            {
                using (var sqlCon = new SqlConnection(_connectionString))
                {
                    await sqlCon.OpenAsync();

                    foreach (var client in clients)
                    {

                        using (var cmd = new SqlCommand(updateQuery, sqlCon))
                        {
                            cmd.Parameters.AddWithValue("@ID_Klienta", client.ClientId);
                            cmd.Parameters.AddWithValue("@Typ_Klienta", client.ClientType ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@Imie", client.FirstName ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@Nazwisko", client.LastName ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@Nazwa_Firmy", client.CompanyName ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@NIP", client.NIP ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@REGON", client.REGON ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@Adres", client.Address ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@Telefon", client.Phone ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@Email", client.Email ?? (object)DBNull.Value);

                            await cmd.ExecuteNonQueryAsync();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Błąd podczas zapisywania klientów: " + ex.Message, ex);
            }
        }

        public async Task AddClientAsync(Client client)
        {
            string insertQuery = @"
        INSERT INTO Klienci (Typ_Klienta, Imie, Nazwisko, Nazwa_Firmy, NIP, REGON, Adres, Telefon, Email)
        VALUES (@Typ_Klienta, @Imie, @Nazwisko, @Nazwa_Firmy, @NIP, @REGON, @Adres, @Telefon, @Email)";

            try
            {
                using (var sqlCon = new SqlConnection(_connectionString))
                {
                    await sqlCon.OpenAsync();

                    using (var cmd = new SqlCommand(insertQuery, sqlCon))
                    {
                        cmd.Parameters.AddWithValue("@Typ_Klienta", client.ClientType ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Imie", client.FirstName ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Nazwisko", client.LastName ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Nazwa_Firmy", client.CompanyName ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@NIP", client.NIP ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@REGON", client.REGON ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Adres", client.Address ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Telefon", client.Phone ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Email", client.Email ?? (object)DBNull.Value);

                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Błąd podczas dodawania klienta: " + ex.Message, ex);
            }
        }
    }
}
