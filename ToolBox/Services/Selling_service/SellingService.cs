using Praca_Inżynierska_v1.Services;
using Praca_Inżynierska_v1.Services.DBConnectionString;

namespace Praca_Inżynierska_v1.MVVM.Model
{
    internal static class SellingService
    {
        internal static List<SellingsInfo> PobierzRaport(DateTime dateFrom, DateTime dateTo)
        {
            string conn = DBConnectionString.GetConnectionString();
            var service = new SellingServiceSql(conn);
            return service.PobierzRaport(dateFrom, dateTo);
        }
    }
}