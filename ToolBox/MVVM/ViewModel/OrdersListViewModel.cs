using Praca_Inżynierska_v1.Core;
using Praca_Inżynierska_v1.Helpers;
using Praca_Inżynierska_v1.Services;
using Praca_Inżynierska_v1.Services.MyMessageBoxEnums;
using Praca_Inżynierska_v1.Services.Order_Service;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Praca_Inżynierska_v1.MVVM.ViewModel
{
    public class OrdersListViewModel : ViewModelBase
    {
        private readonly IOrderService _orderService;

        public ObservableCollection<Order> Orders { get; set; } = new();
        public ObservableCollection<Order> FilteredOrders { get; set; } = new();

        private Order _selectedOrder;

        public bool IsMagazynier =>
            Session.CurrentUser?.Role == "magazynier" ||
            Session.CurrentUser?.Role == "kierownik_magazynu";

        public Order SelectedOrder
        {
            get => _selectedOrder;
            set
            {
                _selectedOrder = value;
                OnPropertyChanged(nameof(SelectedOrder));
            }
        }

        private string _searchKeyword;
        public string SearchKeyword
        {
            get => _searchKeyword;
            set
            {
                _searchKeyword = value;
                OnPropertyChanged(nameof(SearchKeyword));
            }
        }

        public ICommand SearchCommand { get; }

        public OrdersListViewModel(IOrderService orderService)
        {
            _orderService = orderService;

            SearchCommand = new RelayCommand(_ => ExecuteSearch());

            LoadOrders();
        }

        private async void LoadOrders()
        {
            try
            {
                var orders = await _orderService.GetAllOrdersAsync();

                Orders.Clear();
                FilteredOrders.Clear();

                foreach (var order in orders)
                {
                    Orders.Add(order);
                    FilteredOrders.Add(order);
                }
            }
            catch (Exception ex)
            {
                MyMessageBoxService.Show(
                    "Error",
                    $"Failed to load orders:\n{ex.Message}",
                    MyMessageBoxType.Error,
                    MyMessageBoxButtons.OK
                );
            }
        }

        private void ExecuteSearch()
        {
            FilteredOrders.Clear();

            if (string.IsNullOrWhiteSpace(SearchKeyword))
            {
                foreach (var o in Orders)
                    FilteredOrders.Add(o);
            }
            else
            {
                var keyword = SearchKeyword.ToLower();

                foreach (var o in Orders)
                {
                    if (
                        (o.ClientName != null && o.ClientName.ToLower().Contains(keyword)) ||
                        (o.OrderNumber != null && o.OrderNumber.ToLower().Contains(keyword))
                    )
                    {
                        FilteredOrders.Add(o);
                    }
                }
            }
        }

        public async void RefreshOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();

            Orders.Clear();
            FilteredOrders.Clear();

            foreach (var order in orders)
            {
                Orders.Add(order);
                FilteredOrders.Add(order);
            }
        }

        public async Task AssignOrderToCurrentUserAsync()
        {
            if (SelectedOrder == null || SelectedOrder.Status != "Oczekujace")
            {
                MyMessageBoxService.Show("Błąd", "Zamówienie musi być w statusie 'Oczekujące'.", MyMessageBoxType.Error, MyMessageBoxButtons.OK);
                return;
            }

            SelectedOrder.Status = "W realizacji";
            SelectedOrder.UserId = Session.CurrentUser.UserId;

            await _orderService.UpdateOrderAsync(SelectedOrder);
            RefreshOrders();

            MyMessageBoxService.Show("Sukces", "Zamówienie przypisano do Ciebie.", MyMessageBoxType.Success, MyMessageBoxButtons.OK);
        }

        public async Task CompleteOrderAsync()
        {
            if (SelectedOrder == null || SelectedOrder.Status != "W realizacji")
            {
                MyMessageBoxService.Show("Błąd", "Zamówienie musi być w statusie 'W realizacji'.", MyMessageBoxType.Error, MyMessageBoxButtons.OK);
                return;
            }

            SelectedOrder.Status = "Zrealizowane";
            await _orderService.UpdateOrderAsync(SelectedOrder);
            RefreshOrders();

            MyMessageBoxService.Show("Sukces", "Zamówienie oznaczone jako zakończone.", MyMessageBoxType.Success, MyMessageBoxButtons.OK);
        }

    }
}
