using DemoOrders.API.Controllers;
using DemoOrders.Application.Abstration.Service;
using DemoOrders.Application.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace DemoOrders.Tests.Controllers
{
    public class OrderControllerTests
    {
        [Fact]
        public async Task CreateOrder_ReturnsOk_WhenOrderIsCreated()
        {
            // Arrange 
            // 1. Mock the IOrderService
            var orderServiceMock = new Mock<IOrderService>();
            // 2. Setup the mock to return a fake OrderDto when CreateOrderAsync is called
            var testOrderDto = new OrderDto
            {
                Id = 1,
                CustomerName = "Test User",
                Items = new List<OrderItemDto>
                {
                    new OrderItemDto { Id = 1, ProductName = "Test Product", Quantity = 1, Price = 10 }
                }
            };
            // 3. Create an instance of OrderController with the mocked service
            orderServiceMock
                .Setup(s => s.CreateOrderAsync(It.IsAny<CreateOrderDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(testOrderDto);
            // 4. Create the controller instance
            var controller = new OrderController(orderServiceMock.Object, Mock.Of<ILogger<OrderController>>());

            // 5. Create a sample CreateOrderDto
            var createOrderDto = new CreateOrderDto
            {
                CustomerName = "Test User",
                PaymentMethod = 0,
                DeliveryAddress = "Test Address",
                ContactNumber = "123456789",
                Currency = 0,
                Items = new List<CreateOrderItemDto>
                {
                    new CreateOrderItemDto { ProductName = "Test Product", Quantity = 1, Price = 10 }
                }
            };

            // Act
            var result = await controller.CreateOrder(createOrderDto, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedOrder = Assert.IsType<OrderDto>(okResult.Value);
            Assert.Equal("Test User", returnedOrder.CustomerName);
            Assert.Single(returnedOrder.Items);
        }
    }
}



