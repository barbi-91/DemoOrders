using DemoOrders.Application.Abstration.Service;
using DemoOrders.Application.Dtos;
using DemoOrders.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace DemoOrders.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IOrderService orderService, ILogger<OrderController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        // Create a new order
        [HttpPost]
        // api/order/CreateOrder
        [Route("CreateOrder")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto createOrderDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for order creation: {@ModelState}", ModelState);
                return BadRequest(ModelState);
            }

            if (!Enum.IsDefined(typeof(OrderPaymentMethod), createOrderDto.PaymentMethod))
                throw new Exception("Invalid payment method.");

            if (!Enum.IsDefined(typeof(OrderCurrency), createOrderDto.Currency))
                throw new Exception("Invalid currency.");

            try
            {
                var orderDto = await _orderService.CreateOrderAsync(createOrderDto, cancellationToken);
                _logger.LogInformation("Order created successfully for customer {CustomerName} with ID {OrderId}", orderDto.CustomerName, orderDto.Id);
                return Ok(orderDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating order for customer {CustomerName}", createOrderDto.CustomerName);
                return BadRequest(new { errorMessage = "Error has occured!" });
            }
        }

        // Get all orders
        [HttpGet]
        // api/order/GetAllOrders
        [Route("GetAllOrders")]
        public async Task<IActionResult> GetAllOrders(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching all orders.");
                var orders = await _orderService.GetAllOrdersAsync(cancellationToken);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all orders.");
                return BadRequest(new { errorMessage = "Error has occurred while fetching orders!" });
            }
        }
    }
}
