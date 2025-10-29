using DemoOrders.Application.Dtos;

namespace DemoOrders.Application.Abstration.Service
{
    public interface IOrderService
    {
        // Create a new order asynchronously and return the created order with its generated ID, converting between DTOs and entities
        Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto, CancellationToken cancellationToken);

        // Get all orders asynchronously and return a IEnumerable of OrderDto, converting between entities and DTOs
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync(CancellationToken cancellationToken);
    }
}
