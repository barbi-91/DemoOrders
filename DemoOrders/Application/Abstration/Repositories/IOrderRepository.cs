using DemoOrders.Domain.Entities;
using DemoOrders.Domain.Enums;

namespace DemoOrders.Application.Abstration.Repositories
{
    public interface IOrderRepository
    {
        // Create a new order asynchronously and return the created order with its generated ID 
        Task<Order> CreateOrderAsync(Order order, CancellationToken cancellationToken);

        //Get all orders asynchronously and return a IEnumerable of orders
        Task<IEnumerable<Order>> GetAllOrdersAsync(CancellationToken cancellationToken);

        // Update the status of an existing order asynchronously and return the updated order by its ID
        Task<Order?> UpdateOrderStatusAsync(int orderId, OrderStatus status, CancellationToken cancellationToken);

        // Helper method get single method by id
        Task<Order?> GetOrderByIdAsync(int orderId);
    }
}
