using DemoOrders.Application.Abstration.Service;
using DemoOrders.Application.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace DemoOrders.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // Create a new order
        [HttpPost]
        // api/order/CreateOrder
        [Route("CreateOrder")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto createOrderDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var orderDto = await _orderService.CreateOrderAsync(createOrderDto, cancellationToken);
                return Ok(orderDto);
            }
            catch (Exception ex)
            {
                // TODO: log this exception
                return BadRequest(new { errorMessage = "Error has occured!" });
            }
        }
    }
}
