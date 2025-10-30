using AutoMapper;
using DemoOrders.Application.Abstration.Repositories;
using DemoOrders.Application.Abstration.Service;
using DemoOrders.Application.Dtos;
using DemoOrders.Domain.Entities;
using DemoOrders.Domain.Enums;

namespace DemoOrders.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderService> _logger;

        public OrderService(IOrderRepository repository, IMapper mapper, ILogger<OrderService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto, CancellationToken cancellationToken)
        {
            try
            {
                // Validate that the order has at least one item
                if (createOrderDto.Items == null || !createOrderDto.Items.Any())
                    throw new Exception("Order must have at least one item.");

                // Map the CreateOrderDto to an Order entity
                Order createOrder = _mapper.Map<Order>(createOrderDto);

                // Set the initial status of the order to Pending
                createOrder.Status = OrderStatus.Pending;

                // Set the order time to the current time
                createOrder.OrderTime = DateTime.Now;

                // Use the repository to create the order and get the created entity with its generated ID
                Order createdOrder = await _repository.CreateOrderAsync(createOrder, cancellationToken);

                // Map the created Order entity back to an OrderDto
                OrderDto createdOrderDto = _mapper.Map<OrderDto>(createdOrder);
                _logger.LogInformation("Order created successfully with ID {OrderId}", createdOrder.Id);
                return createdOrderDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating order for customer {CustomerName}", createOrderDto.CustomerName);
                throw;
            }
        }

        // Get all orders asynchronously and return a IEnumerable of OrderDto, converting between entities and DTOs
        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching all orders from the database.");
            var orders = await _repository.GetAllOrdersAsync(cancellationToken);
            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        // Update the status of an existing order asynchronously and return the updated OrderDto by its ID, converting between DTOs and entities
        public async Task<OrderDto?> UpdateOrderStatusAsync(int orderId, OrderStatus status, CancellationToken cancellationToken)
        {
            try
            {
                Order? updatedOrder = await _repository.UpdateOrderStatusAsync(orderId, status, cancellationToken);
                if (updatedOrder == null) // Order not found
                {
                    _logger.LogWarning("Order with ID {OrderId} not found for status update.", orderId);
                    return null;
                }
                OrderDto updatedOrderDto = _mapper.Map<OrderDto>(updatedOrder);
                _logger.LogInformation("Order with ID {OrderId} updated successfully to status {Status}.", orderId, status);
                return updatedOrderDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating status for order ID {OrderId}", orderId);
                throw;
            }
        }
    }
}
