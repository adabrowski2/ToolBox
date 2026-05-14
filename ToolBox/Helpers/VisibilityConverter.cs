using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Praca_Inżynierska_v1.Helpers
{
    public class VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool visible = value is bool b && b;
            if (parameter?.ToString() == "invert")
                visible = !visible;

            return visible ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value is Visibility v && v == Visibility.Visible);
        }
    }
}
