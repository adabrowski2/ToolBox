using System.Configuration;

namespace Praca_Inżynierska_v1.Services.DBConnectionString
{
    public static class DBConnectionString
    {
        public static string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["ToolboxDB"].ConnectionString;
        }
    }
}
