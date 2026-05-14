using System.Globalization;
using System.Windows.Controls;

namespace Praca_Inżynierska_v1.Helpers
{
    public class CorrectQuantityValidator : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return new ValidationResult(false, "Wartość wymagana");

            string input = value.ToString().Trim();

            // Obsługuje zarówno przecinek jak i kropkę i zmienia do kropki
            input = input.Replace(',', '.');

            if (decimal.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal quantity))
            {
                return quantity > 0
                    ? ValidationResult.ValidResult
                    : new ValidationResult(false, "Musi być większe niż 0");
            }

            return new ValidationResult(false, "Nieprawidłowa liczba (użyj kropki jako separatora)");
        }
    }
}
