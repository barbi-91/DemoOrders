using DemoOrders.API.Controllers;
using DemoOrders.Application.Abstration.Service;
using DemoOrders.Application.Dtos;
using DemoOrders.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace DemoOrders.Tests.Controllers
{
    public class OrderControllerTests
    {
        // Test for CreateOrder action in OrderController with mocked IOrderService and ILogger and verifying Ok result
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
                PaymentMethod = OrderPaymentMethod.CreditCard,
                DeliveryAddress = "Test Address",
                ContactNumber = "123456789",
                Currency = OrderCurrency.EUR,
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


        // Test for CreateOrder action in OrderController with invalid model state returning BadRequest
        [Fact]
        public async Task CreateOrder_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            // 1. Mock the IOrderService and ILogger
            var orderServiceMock = new Mock<IOrderService>();
            // 2. Create the controller instance
            var loggerMock = new Mock<ILogger<OrderController>>();
            // 3. Create the controller instance
            var controller = new OrderController(orderServiceMock.Object, loggerMock.Object);

            // Simulate invalid model state (e.g. missing required CustomerName)
            controller.ModelState.AddModelError("CustomerName", "Required");

            var createOrderDto = new CreateOrderDto
            {
                // Create an invalid CreateOrderDto (e.g. missing CustomerName)
                DeliveryAddress = "Test Address",
                PaymentMethod = OrderPaymentMethod.Cash,
                ContactNumber = "123456789",
                Currency = OrderCurrency.EUR,
                Items = new List<CreateOrderItemDto>
                {
                    new CreateOrderItemDto { ProductName = "Test", Quantity = 1, Price = 1 }
                }
            };

            // Act
            var result = await controller.CreateOrder(createOrderDto, CancellationToken.None);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }


        // Test for When Service ThrowsException
        [Fact]
        public async Task CreateOrder_ReturnsBadRequest_WhenServiceThrowsException()
        {
            // Arrange
            var orderServiceMock = new Mock<IOrderService>();
            var loggerMock = new Mock<ILogger<OrderController>>();
            var controller = new OrderController(orderServiceMock.Object, loggerMock.Object);

            orderServiceMock
                .Setup(s => s.CreateOrderAsync(It.IsAny<CreateOrderDto>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Service error"));

            var createOrderDto = new CreateOrderDto
            {
                CustomerName = "Test User",
                PaymentMethod = OrderPaymentMethod.Cash,
                DeliveryAddress = "Test Address",
                ContactNumber = "123456789",
                Currency = OrderCurrency.EUR,
                Items = new List<CreateOrderItemDto>
                {
                    new CreateOrderItemDto { ProductName = "Test Product", Quantity = 1, Price = 10 }
                }
            };

            // Act
            var result = await controller.CreateOrder(createOrderDto, CancellationToken.None);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Error has occured", badRequestResult.Value.ToString());
        }


        // Test for error handling when service throws excepti
        [Fact]
        public async Task GetAllOrders_ReturnsBadRequest_WhenServiceThrowsException()
        {
            // Arrange
            var orderServiceMock = new Mock<IOrderService>();
            var loggerMock = new Mock<ILogger<OrderController>>();
            var controller = new OrderController(orderServiceMock.Object, loggerMock.Object);

            // Mock the service to throw an exception
            orderServiceMock
                .Setup(s => s.GetAllOrdersAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new System.Exception("Service error"));

            // Act
            var result = await controller.GetAllOrders(CancellationToken.None);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Error has occurred", badRequestResult.Value.ToString());
        }


        // Test for for empty order list

        [Fact]
        public async Task GetAllOrders_ReturnsOk_WithEmptyList()
        {
            // Arrange
            var orderServiceMock = new Mock<IOrderService>();
            var loggerMock = new Mock<ILogger<OrderController>>();
            var controller = new OrderController(orderServiceMock.Object, loggerMock.Object);

            // Mock the service to return an empty list
            orderServiceMock
                .Setup(s => s.GetAllOrdersAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<OrderDto>());

            // Act
            var result = await controller.GetAllOrders(CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedOrders = Assert.IsAssignableFrom<IEnumerable<OrderDto>>(okResult.Value);
            Assert.Empty(returnedOrders);
        }
    }
}



