using Praca_Inżynierska_v1.Helpers;
using Praca_Inżynierska_v1.MVVM.ViewModel;
using Praca_Inżynierska_v1.Services;
using Praca_Inżynierska_v1.Services.DBConnectionString;
using Praca_Inżynierska_v1.Services.MyMessageBoxEnums;
using Praca_Inżynierska_v1.Services.Order_Service;
using System.Windows;
using System.Windows.Input;

namespace Praca_Inżynierska_v1.MVVM.View
{
    public partial class OrdersList : Window
    {
        public OrdersList()
        {
            InitializeComponent();

            var orderService = new OrderServiceSQL(DBConnectionString.GetConnectionString());
            this.DataContext = new OrdersListViewModel(orderService);
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

        private void AddOrderButton_Click(object sender, RoutedEventArgs e)
        {
            var newOrderWindow = new NewOrderWindow();
            newOrderWindow.ShowDialog();

            if (DataContext is OrdersListViewModel vm)
            {
                vm.RefreshOrders();
            }
        }

        private void DetailsButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is OrdersListViewModel vm && vm.SelectedOrder != null)
            {
                var orderService = new OrderServiceSQL(DBConnectionString.GetConnectionString());
                var detailsWindow = new OrderDetailsWindow(vm.SelectedOrder, orderService);
                detailsWindow.ShowDialog();
            }
            else
            {
                MyMessageBoxService.Show(
                    "Brak wyboru",
                    "Wybierz zamówienie, aby zobaczyć szczegóły.",
                    MyMessageBoxType.Error,
                    MyMessageBoxButtons.OK
                );
            }
        }

        private async void CancelOrderButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is OrdersListViewModel vm && vm.SelectedOrder != null)
            {
                var order = vm.SelectedOrder;

                if (Session.CurrentUser?.Role == "doradca_handlowy")
                {
                    if (!string.Equals(order.Status, "Oczekujace", StringComparison.OrdinalIgnoreCase))
                    {
                        MyMessageBoxService.Show(
                            "Błąd",
                            "Można anulować tylko zamówienia o statusie 'Oczekujące'.",
                            MyMessageBoxType.Error,
                            MyMessageBoxButtons.OK
                        );
                        return;
                    }
                }

                var confirm = MyMessageBoxService.Show(
                    "Potwierdzenie",
                    "Czy na pewno chcesz anulować wybrane zamówienie?",
                    MyMessageBoxType.Question,
                    MyMessageBoxButtons.YesNo
                );

                if (confirm == MyMessageBoxResult.Yes)
                {
                    try
                    {
                        var orderService = new OrderServiceSQL(DBConnectionString.GetConnectionString());
                        await orderService.DeleteOrderAsync(order.OrderId);
                        vm.RefreshOrders();

                        MyMessageBoxService.Show(
                            "Sukces",
                            "Zamówienie zostało anulowane.",
                            MyMessageBoxType.Success,
                            MyMessageBoxButtons.OK
                        );
                    }
                    catch (Exception ex)
                    {
                        MyMessageBoxService.Show(
                            "Błąd",
                            $"Wystąpił problem przy anulowaniu zamówienia:\n{ex.Message}",
                            MyMessageBoxType.Error,
                            MyMessageBoxButtons.OK
                        );
                    }
                }
            }
            else
            {
                MyMessageBoxService.Show(
                    "Brak wyboru",
                    "Wybierz zamówienie do anulowania.",
                    MyMessageBoxType.Error,
                    MyMessageBoxButtons.OK
                );
            }
        }

        private async void AssignOrderButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is OrdersListViewModel vm && vm.SelectedOrder != null)
            {
                await vm.AssignOrderToCurrentUserAsync();
            }
        }

        private async void CompleteOrderButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is OrdersListViewModel vm && vm.SelectedOrder != null)
            {
                await vm.CompleteOrderAsync();
            }
        }
    }
}