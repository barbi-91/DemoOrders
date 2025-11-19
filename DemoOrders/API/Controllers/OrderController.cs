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
                if (createOrderDto.Items == null || !createOrderDto.Items.Any())
                {
                    _logger.LogWarning("Attempted to create order with no items for customer {CustomerName}", createOrderDto.CustomerName);
                    return BadRequest(new { errorMessage = "Order must have at least one item." });
                }
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
                if (orders == null || !orders.Any())
                {
                    _logger.LogInformation("No orders found in the database.");
                    return NotFound(new { message = "No orders found." });
                }
                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all orders.");
                return BadRequest(new { errorMessage = "Error has occurred while fetching orders!" });
            }
        }

        // Update order status by Id 
        [HttpPatch]
        // api/order/UpdateOrderStatus/3/status
        [Route("UpdateOrderStatus/{orderId}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, [FromBody] OrderStatus orderStatus, CancellationToken cancellationToken)
        {
            try
            {
                if (ModelState.IsValid == false)
                {
                    _logger.LogWarning("Invalid model state for updating order status: {@ModelState}", ModelState);
                    return BadRequest(ModelState);
                }
                // Validate order status value 
                if (!Enum.IsDefined(typeof(OrderStatus), orderStatus))
                {
                    _logger.LogWarning("Invalid order status value: {OrderStatus}", orderStatus);
                    return BadRequest(new { message = "Invalid order status." });
                }
                // Update the order status
                OrderDto? updatedOrder = await _orderService.UpdateOrderStatusAsync(orderId, orderStatus, cancellationToken);
                if (updatedOrder == null)
                {
                    _logger.LogWarning("Order with ID {OrderId} not found for status update.", orderId);
                    return NotFound(new { message = "Order not found." });
                }
                _logger.LogInformation("Order ID {OrderId} status updated to {OrderStatus}.", orderId, orderStatus);
                return Ok(updatedOrder);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating status for order ID {OrderId}", orderId);
                return BadRequest(new { errorMessage = "Error has occurred while updating order status!" });
            }
        }

        // Get Total Amount by order id 
        [HttpGet]
        // api/order/GetTotalAmountByOrderId/3
        [Route("GetTotalAmountByOrderId/{orderId}")]
        public async Task<IActionResult> GetTotalAmountByOrderId(int orderId, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Calculating total amount for order ID {OrderId}.", orderId);
                decimal totalAmount = await _orderService.GetTotalAmountByOrderIdAsync(orderId, cancellationToken);
                if (totalAmount == 0m)
                {
                    _logger.LogWarning("Order with ID {OrderId} not found or has no items.", orderId);
                    return NotFound(new { message = "Order not found or has no items." });
                }
                return Ok(new { OrderId = orderId, TotalAmount = totalAmount });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while calculating total amount for order ID {OrderId}", orderId);
                return BadRequest(new { errorMessage = "Error has occurred while calculating total amount!" });
            }
        }

        // Sort orders by Total Amount 
        [HttpGet("sorted-by-total")]
        // api/order/GetOrdersSortedByTotalAmount
        public async Task<IActionResult> GetOrdersSortedByTotalAmount(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching orders sorted by total amount.");
                IEnumerable<OrderDto> sortedOrders = await _orderService.GetOrdersSortedByTotalAmountAsync(cancellationToken);
                if (sortedOrders == null || !sortedOrders.Any())
                {
                    _logger.LogInformation("No orders found to sort by total amount.");
                    return NotFound(new { message = "No orders found." });
                }
                return Ok(sortedOrders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching orders sorted by total amount.");
                return BadRequest(new { errorMessage = "Error has occurred while fetching sorted orders!" });
            }
        }
    }
}

