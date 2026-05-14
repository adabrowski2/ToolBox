using Praca_Inżynierska_v1.MVVM.Model;
using Praca_Inżynierska_v1.Services.Order_Service;
using System.Collections.ObjectModel;

namespace Praca_Inżynierska_v1.MVVM.ViewModel
{
    public class OrderDetailsViewModel : ViewModelBase
    {
        public Order Order { get; }
        public ObservableCollection<OrderItem> Items { get; } = new();

        public OrderDetailsViewModel(Order order, IOrderService orderService)
        {
            Order = order;

            _ = LoadItemsAsync(orderService);
        }

        private async Task LoadItemsAsync(IOrderService orderService)
        {
            var items = await orderService.GetOrderItemsAsync(Order.OrderId);
            Items.Clear();
            foreach (var item in items)
                Items.Add(item);
        }
    }
}
