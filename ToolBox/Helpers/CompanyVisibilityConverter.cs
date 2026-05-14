using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Praca_Inżynierska_v1.Helpers
{
    public class CompanyVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string clientType)
            {
                return clientType == "Jednoosobowa dzialalnosc gospodarcza"
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
