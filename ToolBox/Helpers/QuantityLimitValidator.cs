using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using Praca_Inżynierska_v1.MVVM.Model;

namespace Praca_Inżynierska_v1.Helpers
{
    public class QuantityLimitValidator : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var bindingGroup = value as BindingGroup;
            if (bindingGroup == null || bindingGroup.Items.Count == 0)
                return new ValidationResult(false, "Nie można odczytać danych.");

            var orderItem = bindingGroup.Items[0] as OrderItem;
            if (orderItem == null)
                return new ValidationResult(false, "Nie można odczytać pozycji.");

            var quantityInput = bindingGroup.GetValue(orderItem, nameof(orderItem.Quantity))?.ToString();
            if (string.IsNullOrWhiteSpace(quantityInput))
                return new ValidationResult(false, "Wpisz ilość.");

            quantityInput = quantityInput.Replace(',', '.');

            if (!decimal.TryParse(quantityInput, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal quantity))
                return new ValidationResult(false, "Niepoprawna ilość (użyj kropki jako separatora)");

            if (quantity <= 0)
                return new ValidationResult(false, "Ilość musi być większa niż 0.");

            if (quantity > orderItem.StockQuantity)
                return new ValidationResult(false, $"Brak wystarczającej ilości w magazynie (stan: {orderItem.StockQuantity}).");

            return ValidationResult.ValidResult;
        }
    }
}
