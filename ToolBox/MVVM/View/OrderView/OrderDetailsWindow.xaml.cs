using Praca_Inżynierska_v1.MVVM.Model;
using Praca_Inżynierska_v1.Services.Order_Service;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;

namespace Praca_Inżynierska_v1.MVVM.View
{
    public partial class OrderDetailsWindow : Window
    {
        public ObservableCollection<OrderItem> OrderItems { get; set; } = new();
        public string OrderHeader { get; set; }

        public OrderDetailsWindow(Order order, IOrderService orderService)
        {
            InitializeComponent();
            DataContext = this;

            OrderHeader = $"Zamówienie: {order.OrderNumber} — {order.ClientName}";

            LoadItemsAsync(order.OrderId, orderService);
        }

        private async void LoadItemsAsync(int orderId, IOrderService orderService)
        {
            var items = await orderService.GetOrderItemsAsync(orderId);
            foreach (var item in items)
            {
                OrderItems.Add(item);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
