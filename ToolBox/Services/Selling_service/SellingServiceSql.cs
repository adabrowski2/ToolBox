using System.Data;
using Microsoft.Data.SqlClient;
using Praca_Inżynierska_v1.MVVM.Model;

namespace Praca_Inżynierska_v1.Services
{
    internal class SellingServiceSql
    {
        private readonly string _connectionString;

        public SellingServiceSql(string connectionString)
        {
            _connectionString = connectionString;
        }

        internal List<SellingsInfo> PobierzRaport(DateTime dataOd, DateTime dataDo)
        {
            var wyniki = new List<SellingsInfo>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("PobierzRaportSprzedazy", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DataOd", dataOd);
                cmd.Parameters.AddWithValue("@DataDo", dataDo);

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        wyniki.Add(new SellingsInfo
                        {
                            Produkt = reader["Produkt"].ToString(),
                            Ilosc = Convert.ToInt32(reader["Ilosc"]),
                            WartoscDecimal = Convert.ToDecimal(reader["Wartosc"]),
                            Wartosc = string.Format("{0:N2}", reader["Wartosc"]) + " zł",
                            CenaJednostkowa = Convert.ToDecimal(reader["CenaJednostkowa"]),
                            Rabat = reader["Rabat"] != DBNull.Value ? Convert.ToDecimal(reader["Rabat"]) : (decimal?)null,
                            LiczbaZamowien = Convert.ToInt32(reader["LiczbaZamowien"])
                        });
                    }
                }
            }

            return wyniki;
        }
    }
}