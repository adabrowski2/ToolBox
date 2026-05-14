using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using Praca_Inżynierska_v1.Services;
using Praca_Inżynierska_v1.Services.MyMessageBoxEnums;

namespace Praca_Inżynierska_v1.MVVM.Model
{
    public class OrderItem : INotifyPropertyChanged
    {
        private int _productId;
        private string _productName;
        private string _unit;
        private decimal _stockQuantity;
        private decimal _quantity;
        private decimal _unitPrice;
        private decimal? _discount;
        private decimal _totalPrice;

        public int OrderItemId { get; set; }
        public int OrderId { get; set; }

        public int ProductId
        {
            get => _productId;
            set { _productId = value; OnPropertyChanged(); }
        }

        public string ProductName
        {
            get => _productName;
            set { _productName = value; OnPropertyChanged(); }
        }

        public string Unit
        {
            get => _unit;
            set { _unit = value; OnPropertyChanged(); }
        }

        public decimal StockQuantity
        {
            get => _stockQuantity;
            set { _stockQuantity = value; OnPropertyChanged(); }
        }

        public decimal Quantity
        {
            get => _quantity;
            set
            {
                _quantity = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(QuantityRaw)); // synchronizuj
                UpdateTotalPrice();
            }
        }

        public string QuantityRaw
        {
            get => Quantity.ToString(CultureInfo.CurrentCulture);
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    return;

                var normalized = value.Replace(',', '.');

                if (!decimal.TryParse(normalized, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal parsed))
                {
                    ShowError("Nieprawidłowa wartość. Wpisz liczbę.");
                    return;
                }

                if (Unit?.ToLower().Contains("szt") == true && parsed != Math.Floor(parsed))
                {
                    ShowError("Dla jednostek 'sztuka' dozwolone są tylko liczby całkowite.");
                    return;
                }

                if (parsed <= 0)
                {
                    ShowError("Ilość musi być większa niż 0.");
                    return;
                }

                if (parsed > StockQuantity)
                {
                    ShowError($"Brak wystarczającej ilości w magazynie (stan: {StockQuantity}).");
                    return;
                }

                Quantity = parsed;
            }
        }

        public decimal UnitPrice
        {
            get => _unitPrice;
            set
            {
                if (_unitPrice != value)
                {
                    _unitPrice = value;
                    OnPropertyChanged();
                    UpdateTotalPrice();
                }
            }
        }

        public decimal? Discount
        {
            get => _discount;
            set { _discount = value; OnPropertyChanged(); }
        }

        public DateTime? DiscountStartDate { get; set; }
        public DateTime? DiscountEndDate { get; set; }

        public decimal TotalPrice
        {
            get => _totalPrice;
            set { _totalPrice = value; OnPropertyChanged(); }
        }

        private void UpdateTotalPrice()
        {
            TotalPrice = Math.Round(Quantity * UnitPrice, 2);
        }

        private void ShowError(string message)
        {
            MyMessageBoxService.Show("Błąd", message, MyMessageBoxType.Error, MyMessageBoxButtons.OK);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}