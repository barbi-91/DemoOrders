using DemoOrders.Application.Dtos;
using DemoOrders.Domain.Enums;

namespace DemoOrders.Application.Abstration.Service
{
    public interface IOrderService
    {
        // Create a new order asynchronously and return the created order with its generated ID, converting between DTOs and entities
        Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto, CancellationToken cancellationToken);

        // Get all orders asynchronously and return a IEnumerable of OrderDto, converting between entities and DTOs
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync(CancellationToken cancellationToken);

        // Update the status of an existing order asynchronously and return the updated OrderDto by its ID, converting between DTOs and entities
        Task<OrderDto?> UpdateOrderStatusAsync(int orderId, OrderStatus status, CancellationToken cancellationToken);

        // Get Total Amount by order id 
        Task<decimal> GetTotalAmountByOrderIdAsync(int orderId, CancellationToken cancellationToken);
    }
}
