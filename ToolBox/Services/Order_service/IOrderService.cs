using Praca_Inżynierska_v1.MVVM.Model;

namespace Praca_Inżynierska_v1.Services.Order_Service
{
    public interface IOrderService
    {
        Task<List<Order>> GetAllOrdersAsync();
        Task<List<OrderItem>> GetOrderItemsAsync(int orderId);
        Task AddOrderAsync(Order order, List<OrderItem> items);
        Task UpdateOrderAsync(Order order);
        Task DeleteOrderAsync(int orderId);
    }
}
