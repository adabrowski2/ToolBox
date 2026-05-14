using System.Windows;
using Praca_Inżynierska_v1.MVVM.Model;
using Praca_Inżynierska_v1.Services;
using Praca_Inżynierska_v1.Services.Discount_Service;
using Praca_Inżynierska_v1.Services.DBConnectionString;
using Praca_Inżynierska_v1.Services.MyMessageBoxEnums;

namespace Praca_Inżynierska_v1.MVVM.View
{
    public partial class SetDiscountView : Window
    {
        private readonly Product _product;

        public SetDiscountView(Product product)
        {
            InitializeComponent();
            _product = product;
            EndDatePicker.SelectedDate = DateTime.Today.AddDays(7);
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (!decimal.TryParse(DiscountTextBox.Text, out decimal discount) || discount < 0 || discount > 100)
            {
                MyMessageBoxService.Show(
                    "Błąd",
                    "Wprowadź poprawną wartość rabatu (0–100%).",
                    MyMessageBoxType.Error,
                    MyMessageBoxButtons.OK,
                    this);
                return;
            }

            if (EndDatePicker.SelectedDate == null || EndDatePicker.SelectedDate <= DateTime.Today)
            {
                MyMessageBoxService.Show(
                    "Błąd",
                    "Wprowadź poprawną datę ważności rabatu.",
                    MyMessageBoxType.Error,
                    MyMessageBoxButtons.OK,
                    this);
                return;
            }

            try
            {
                var discountService = new DiscountService(DBConnectionString.GetConnectionString());
                discountService.SaveDiscount(_product.ProductId, discount, EndDatePicker.SelectedDate.Value);

                MyMessageBoxService.Show(
                    "Sukces",
                    $"Rabat {discount}% ustawiony do {EndDatePicker.SelectedDate:dd.MM.yyyy}",
                    MyMessageBoxType.Success,
                    MyMessageBoxButtons.OK,
                    this);
            }
            catch (Exception ex)
            {
                MyMessageBoxService.Show(
                    "Błąd",
                    "Nie udało się zapisać rabatu: " + ex.Message,
                    MyMessageBoxType.Error,
                    MyMessageBoxButtons.OK,
                    this);
            }

            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}