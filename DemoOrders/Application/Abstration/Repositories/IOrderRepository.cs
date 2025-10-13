using DemoOrders.Domain.Entities;

namespace DemoOrders.Application.Abstration.Repositories
{
    public interface IOrderRepository
    {
        // Create a new order asynchronously and return the created order with its generated ID 
        Task<Order> CreateOrderAsync(Order order, CancellationToken cancellationToken);
    }
}
