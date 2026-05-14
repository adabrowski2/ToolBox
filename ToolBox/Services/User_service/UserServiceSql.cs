using Microsoft.Data.SqlClient;
using System.Text;
using System.Security.Cryptography;
using Praca_Inżynierska_v1.MVVM.Model;

namespace Praca_Inżynierska_v1.Services.User_Service
{
    internal class UserServiceSql : IUserService
    {
        private readonly string _connectionString;

        public UserServiceSql(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<bool> ValidateUserAsync(string username, string password)
        {
            // pobieramy hash i salt z bazy, porównujemy z hashem wprowadzonego hasła
            try
            {
                using (var sqlCon = new SqlConnection(_connectionString))
                {
                    await sqlCon.OpenAsync();

                    string query = "SELECT Haslo, Salt FROM dbo.Uzytkownicy WHERE Login = @Login";
                    using (var sqlCmd = new SqlCommand(query, sqlCon))
                    {
                        sqlCmd.Parameters.AddWithValue("@Login", username);

                        using (var reader = await sqlCmd.ExecuteReaderAsync())
                        {
                            if (reader.Read())
                            {
                                string storedHash = reader["Haslo"].ToString();
                                string storedSalt = reader["Salt"].ToString();

                                // Hashujemy hasło wprowadzone przez użytkownika:
                                string inputHash = HashPassword(password, storedSalt);

                                // Porównujemy z tym w bazie
                                return storedHash == inputHash;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Obsługa błędów (logowanie, throw, etc.)
                return false;
            }

            // Jeśli nie znaleziono użytkownika lub wystąpił błąd – zwracamy false.
            return false;
        }

        public async Task<User> GetUserByCredentialsAsync(string login, string password)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            // pobieramy użytkownika i jego sól
            var command = new SqlCommand(
                "SELECT * FROM Uzytkownicy WHERE Login = @login", connection);
            command.Parameters.AddWithValue("@login", login);

            using var reader = await command.ExecuteReaderAsync();
            if (!reader.HasRows)
                return null;

            await reader.ReadAsync();

            string storedHash = reader["Haslo"].ToString();
            string storedSalt = reader["Salt"].ToString();

            string inputHash = HashPassword(password, storedSalt);

            if (storedHash != inputHash)
                return null;

            return new User
            {
                UserId = Convert.ToInt32(reader["ID_Uzytkownika"]),
                Login = reader["Login"].ToString(),
                FirstName = reader["Imie"].ToString(),
                LastName = reader["Nazwisko"].ToString(),
                Role = reader["Rola"].ToString(),
                LastLoginDate = reader["Data_ostatniego_logowania"] == DBNull.Value
                                ? null
                                : Convert.ToDateTime(reader["Data_ostatniego_logowania"])
            };
        }

        // Metoda do hashowania
        private string HashPassword(string password, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                var combinedBytes = Encoding.UTF8.GetBytes(password + salt);
                var hashBytes = sha256.ComputeHash(combinedBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }
    }
}
