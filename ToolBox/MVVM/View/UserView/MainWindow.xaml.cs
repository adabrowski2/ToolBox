using Praca_Inżynierska_v1.Helpers;
using Praca_Inżynierska_v1.MVVM.View;
using Praca_Inżynierska_v1.Services;
using Praca_Inżynierska_v1.Services.DBConnectionString;
using Praca_Inżynierska_v1.Services.Discount_Service;
using Praca_Inżynierska_v1.Services.MyMessageBoxEnums;
using System.Windows;
using System.Windows.Input;

namespace Praca_Inżynierska_v1
{
    public partial class MainWindow : Window
    {


        public MainWindow()
        {
            InitializeComponent();
            string connStr = DBConnectionString.GetConnectionString();
            var discountService = new DiscountService(connStr);
            discountService.DiscountExpire();

            // Ukrywanie przycisków
            if (Session.CurrentUser?.Role == "kierownik_dzialu_handlowego")
            {
                SalesStatsButton.Visibility = Visibility.Visible;
            }

            if (Session.CurrentUser?.Role == "kierownik_dzialu_handlowego")
            {
                SalesStatsButton.Visibility = Visibility.Visible;
            }
            else if (Session.CurrentUser?.Role == "kierownik_magazynu")
            {
                OrderDeliveryButton.Visibility = Visibility.Visible;
                DeliveryListButton.Visibility = Visibility.Visible;
            }

        }

        private void ProductsList_Click(object sender, RoutedEventArgs e)
        {
            var window = new MVVM.View.ProductsList();
            window.ShowDialog();
        }

        private void ClientsList_Click(object sender, RoutedEventArgs e)
        {
            var window = new MVVM.View.ClientsList();
            window.ShowDialog();
        }

        private void OrdersList_Click(object sender, RoutedEventArgs e)
        {
            var window = new MVVM.View.OrdersList();
            window.ShowDialog();
        }

        private void SalesStatsButton_Click(object sender, RoutedEventArgs e)
        {
            if (Session.CurrentUser?.Role == "kierownik_dzialu_handlowego")
            {
                var statsWindow = new StatisticsView();
                statsWindow.ShowDialog();
            }
            else
            {
                MyMessageBoxService.Show("Brak dostępu", "Nie masz uprawnień do przeglądania statystyk.", MyMessageBoxType.Error, MyMessageBoxButtons.OK);
            }
        }

        private void LogOut_Click(object sender, RoutedEventArgs e)
        {
            var result = MyMessageBoxService.Show(
                "Wylogowanie",
                "Czy na pewno chcesz się wylogować?",
                MyMessageBoxType.Question,
                MyMessageBoxButtons.YesNo,
                this
            );

            if (result == MyMessageBoxResult.Yes)
            {
                var loginWindow = new Login();
                loginWindow.Show();
                this.Close();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MyMessageBoxService.Show(
                "Wyjście",
                "Czy na pewno chcesz wyjść?",
                MyMessageBoxType.Question,
                MyMessageBoxButtons.YesNo,
                this
            );

            if (result == MyMessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }

        private void MinimazeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowHelpers.Minimize(this);
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowHelpers.MaximizeOrRestore(this);
        }

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            WindowHelpers.DragWindow(sender, e);
        }

        private void OrderDeliveryButton_Click(object sender, RoutedEventArgs e)
        {
            var deliveryWindow = new DeliveryOrderView();
            deliveryWindow.ShowDialog();
        }

        private void OpenDeliveryList_Click(object sender, RoutedEventArgs e)
        {
            var deliveryListView = new DeliveryListView();
            deliveryListView.ShowDialog();
        }

    }
}