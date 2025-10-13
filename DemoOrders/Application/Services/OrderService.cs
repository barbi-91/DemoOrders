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

        public OrderService(IOrderRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto, CancellationToken cancellationToken)
        {
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
            return createdOrderDto;
        }
    }
}
