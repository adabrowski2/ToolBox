using Praca_Inżynierska_v1.Helpers;
using Praca_Inżynierska_v1.MVVM.Model;
using Praca_Inżynierska_v1.MVVM.ViewModel;
using Praca_Inżynierska_v1.Services;
using Praca_Inżynierska_v1.Services.DBConnectionString;
using Praca_Inżynierska_v1.Services.Discount_Service;
using Praca_Inżynierska_v1.Services.MyMessageBoxEnums;
using Praca_Inżynierska_v1.Services.Product_Service;
using System.Windows;
using System.Windows.Input;

namespace Praca_Inżynierska_v1.MVVM.View
{
    public partial class ProductsList : Window
    {
        public ProductsList()
        {
            InitializeComponent();

            string connectionString = DBConnectionString.GetConnectionString();
            var productService = new ProductServiceSql(connectionString);
            var vm = new ProductsListViewModel(productService);
            this.DataContext = vm;

            if (Session.CurrentUser?.Role == "kierownik_dzialu_handlowego")
            {
                SetDiscountButton.Visibility = Visibility.Visible;
                RemoveDiscountButton.Visibility = Visibility.Visible;
            }

        }

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            WindowHelpers.DragWindow(this, e);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            WindowHelpers.Close(this);
        }

        private void MinimazeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowHelpers.Minimize(this);
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowHelpers.MaximizeOrRestore(this);
        }

        internal void SetDiscountButton_Click(object sender, RoutedEventArgs e)
        {
            if (ProductDataGrid.SelectedItem is Product selectedProduct)
            {
                var discountWindow = new SetDiscountView(selectedProduct);
                discountWindow.ShowDialog();
                var vm = this.DataContext as ProductsListViewModel;
                vm?.LoadProducts();
            }
            else
            {
                MyMessageBoxService.Show(
                    "Brak wyboru",
                    "Zaznacz produkt, dla którego chcesz ustalić rabat.",
                    MyMessageBoxType.Info,
                    MyMessageBoxButtons.OK,
                    this);
            }
        }

        internal void RemoveDiscountButton_Click(object sender, RoutedEventArgs e)
        {
            if (ProductDataGrid.SelectedItem is Product selectedProduct)
            {
                var result = MyMessageBoxService.Show(
                    "Potwierdzenie",
                    $"Czy na pewno chcesz usunąć rabat dla produktu: {selectedProduct.Name}?",
                    MyMessageBoxType.Error,
                    MyMessageBoxButtons.YesNo,
                    this);

                if (result == MyMessageBoxResult.Yes)
                {
                    string connectionString = DBConnectionString.GetConnectionString();
                    var discountService = new DiscountService(connectionString);

                    try
                    {
                        discountService.DeactivateDiscount(selectedProduct.ProductId);

                        var vm = this.DataContext as ProductsListViewModel;
                        vm?.LoadProducts();

                        MyMessageBoxService.Show(
                            "Sukces",
                            "Rabat został usunięty.",
                            MyMessageBoxType.Success,
                            MyMessageBoxButtons.OK,
                            this);
                    }
                    catch (Exception ex)
                    {
                        MyMessageBoxService.Show(
                            "Błąd",
                            $"Wystąpił problem podczas usuwania rabatu:\n{ex.Message}",
                            MyMessageBoxType.Error,
                            MyMessageBoxButtons.OK,
                            this);
                    }
                }
            }
            else
            {
                MyMessageBoxService.Show(
                    "Brak wyboru",
                    "Zaznacz produkt, dla którego chcesz usunąć rabat.",
                    MyMessageBoxType.Info,
                    MyMessageBoxButtons.OK,
                    this);
            }
        }

    }
}