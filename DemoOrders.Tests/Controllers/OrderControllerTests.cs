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


        // Test for CreateOrder when Service Throws Exception
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


        // Test in GetAllOrders action for error handling when service throws exception 
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


        // Test in GetAllOrders action for empty order list

        [Fact]
        public async Task GetAllOrders_ReturnsNotFoundResult_WithEmptyList()
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
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("No orders found.", notFoundResult.Value.GetType().GetProperty("message")?.GetValue(notFoundResult.Value));
        }


        // Test in GetAllOrders action for testing successful retrieval with status Ok 
        [Fact]
        public async Task GetAllOrders_ReturnsOk_WithOrders()
        {
            // Arrange
            // 1. Mock the IOrderService
            var orderServiceMock = new Mock<IOrderService>();
            // 2. Mock the logger for OrderController 
            var loggerMock = new Mock<ILogger<OrderController>>();
            // 3. Create an instance of OrderController with the mocked service and logger
            var controller = new OrderController(orderServiceMock.Object, loggerMock.Object);
            // 4. Setup the mock to return a list of OrderDto when GetAllOrdersAsync is called
            var ordersList = new List<OrderDto>
            {
                new OrderDto
                {
                    Id = 1,
                    CustomerName = "Test User 1",
                    Items = new List<OrderItemDto>
                    {
                        new OrderItemDto { Id = 1, ProductName = "Product 1", Quantity = 2, Price = 50 }
                    }
                },
                new OrderDto
                {
                    Id = 2,
                    CustomerName = "Test User 2",
                    Items = new List<OrderItemDto>
                    {
                        new OrderItemDto { Id = 2, ProductName = "Product 2", Quantity = 1, Price = 30 }
                    }
                }
            };
            // Set up the mock to return the orders list
            orderServiceMock
                .Setup(s => s.GetAllOrdersAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(ordersList);

            // Act
            var result = await controller.GetAllOrders(CancellationToken.None);

            // Assert
            // 1. Verify that the result is OkObjectResult
            var okResult = Assert.IsType<OkObjectResult>(result);
            // 2. Verify that the returned value is a list of OrderDto with correct data
            var returnedOrders = Assert.IsAssignableFrom<IEnumerable<OrderDto>>(okResult.Value);
            // 3. Verify the contents of the returned orders
            Assert.Equal(2, returnedOrders.Count());
            // 4. Verify specific order details
            Assert.Contains(returnedOrders, o => o.CustomerName == "Test User 1");
            // 5. Verify specific order details
            Assert.Contains(returnedOrders, o => o.CustomerName == "Test User 2");
        }

        // Test for UpdateOrderStatus action in OrderController with mocked IOrderService and ILogger and verifying Ok result
        [Fact]
        public async Task UpdateOrderStatus_ReturnsOk_WhenOrderIsUpdated()
        {
            // Arrange 
            var orderServiceMock = new Mock<IOrderService>();
            var loggerMock = new Mock<ILogger<OrderController>>();
            var controller = new OrderController(orderServiceMock.Object, loggerMock.Object);
            var updatedOrder = new OrderDto
            {
                Id = 1,
                CustomerName = "Updated User Test",
                Status = OrderStatus.Completed,
                Items = new List<OrderItemDto>
                {
                    new OrderItemDto { Id = 1, ProductName = "Updated Product", Quantity = 2, Price = 20 }
                }
            };

            orderServiceMock
                .Setup(s => s.UpdateOrderStatusAsync(1, OrderStatus.Completed, It.IsAny<CancellationToken>()))
                .ReturnsAsync(updatedOrder);

            // Act
            var result = await controller.UpdateOrderStatus(1, OrderStatus.Completed, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedOrder = Assert.IsType<OrderDto>(okResult.Value);
            Assert.Equal(OrderStatus.Completed, returnedOrder.Status);
        }

        // Test for UpdateOrderStatus action in OrderController returning NotFound when order does not exist
        [Fact]
        public async Task UpdateOrderStatus_ReturnsNotFound_WhenOrderDoesNotExist()
        {
            // Arrange
            var orderServiceMock = new Mock<IOrderService>();
            var loggerMock = new Mock<ILogger<OrderController>>();
            var controller = new OrderController(orderServiceMock.Object, loggerMock.Object);

            orderServiceMock
                .Setup(s => s.UpdateOrderStatusAsync(1, OrderStatus.Completed, It.IsAny<CancellationToken>()))
                .ReturnsAsync((OrderDto?)null);

            // Act
            var result = await controller.UpdateOrderStatus(1, OrderStatus.Completed, CancellationToken.None);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Order not found.", notFoundResult.Value.GetType().GetProperty("message")?.GetValue(notFoundResult.Value));
        }


        // Test for UpdateOrderStatus action in OrderController with invalid model state returning BadRequest
        [Fact]
        public async Task UpdateOrderStatus_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var orderServiceMock = new Mock<IOrderService>();
            var loggerMock = new Mock<ILogger<OrderController>>();
            var controller = new OrderController(orderServiceMock.Object, loggerMock.Object);

            controller.ModelState.AddModelError("Status", "Required");

            // Act
            var result = await controller.UpdateOrderStatus(1, OrderStatus.Completed, CancellationToken.None);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        // Test for GetTotalAmountByOrderId action in OrderController returning Ok with total amount
        [Fact]
        public async Task GetTotalAmountByOrderId_ReturnsOk_WhenOrderExists()
        {
            // Arrange
            var orderServiceMock = new Mock<IOrderService>();
            var loggerMock = new Mock<ILogger<OrderController>>();
            var controller = new OrderController(orderServiceMock.Object, loggerMock.Object);

            int orderId = 1;
            decimal totalAmount = 150.75m;

            orderServiceMock
                .Setup(s => s.GetTotalAmountByOrderIdAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(totalAmount);

            // Act
            var result = await controller.GetTotalAmountByOrderId(orderId, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = okResult.Value;
            Assert.Equal(orderId, (int)value.GetType().GetProperty("OrderId")?.GetValue(value));
            Assert.Equal(totalAmount, (decimal)value.GetType().GetProperty("TotalAmount")?.GetValue(value));
        }

        // Test for GetTotalAmountByOrderId action in OrderController returning NotFound when order does not exist or has no items
        [Fact]
        public async Task GetTotalAmountByOrderId_ReturnsNotFound_WhenOrderDoesNotExistOrHasNoItems()
        {
            // Arrange
            var orderServiceMock = new Mock<IOrderService>();
            var loggerMock = new Mock<ILogger<OrderController>>();
            var controller = new OrderController(orderServiceMock.Object, loggerMock.Object);

            int orderId = 99;
            orderServiceMock
                .Setup(s => s.GetTotalAmountByOrderIdAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(0m);

            // Act
            var result = await controller.GetTotalAmountByOrderId(orderId, CancellationToken.None);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Order not found or has no items.", notFoundResult.Value.GetType().GetProperty("message")?.GetValue(notFoundResult.Value));
        }


        //  Test for GetTotalAmountByOrderId action in OrderController returning BadRequest when service throws exception
        [Fact]
        public async Task GetTotalAmountByOrderId_ReturnsBadRequest_WhenServiceThrowsException()
        {
            // Arrange
            var orderServiceMock = new Mock<IOrderService>();
            var loggerMock = new Mock<ILogger<OrderController>>();
            var controller = new OrderController(orderServiceMock.Object, loggerMock.Object);

            int orderId = 1;
            orderServiceMock
                .Setup(s => s.GetTotalAmountByOrderIdAsync(orderId, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Service error"));

            // Act
            var result = await controller.GetTotalAmountByOrderId(orderId, CancellationToken.None);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Error has occurred while calculating total amount", badRequestResult.Value.ToString());
        }

        // Test for GetOrdersSortedByTotalAmount action in OrderController returning Ok with sorted orders
        [Fact]
        public async Task GetOrdersSortedByTotalAmount_ReturnsOk_WhenOrdersExist()
        {
            // Arrange
            var orderServiceMock = new Mock<IOrderService>();
            var loggerMock = new Mock<ILogger<OrderController>>();
            var controller = new OrderController(orderServiceMock.Object, loggerMock.Object);

            var sortedOrders = new List<OrderDto>
            {
                new OrderDto { Id = 1, CustomerName = "A", Items = new List<OrderItemDto>() },
                new OrderDto { Id = 2, CustomerName = "B", Items = new List<OrderItemDto>() }
            };

            orderServiceMock
                .Setup(s => s.GetOrdersSortedByTotalAmountAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(sortedOrders);

            // Act
            var result = await controller.GetOrdersSortedByTotalAmount(CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedOrders = Assert.IsAssignableFrom<IEnumerable<OrderDto>>(okResult.Value);
            Assert.Equal(2, returnedOrders.Count());
        }

        // Test for GetOrdersSortedByTotalAmount action in OrderController returning NotFound when no orders exist
        [Fact]
        public async Task GetOrdersSortedByTotalAmount_ReturnsNotFound_WhenNoOrdersExist()
        {
            // Arrange
            var orderServiceMock = new Mock<IOrderService>();
            var loggerMock = new Mock<ILogger<OrderController>>();
            var controller = new OrderController(orderServiceMock.Object, loggerMock.Object);

            orderServiceMock
                .Setup(s => s.GetOrdersSortedByTotalAmountAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<OrderDto>());

            // Act
            var result = await controller.GetOrdersSortedByTotalAmount(CancellationToken.None);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("No orders found.", notFoundResult.Value.GetType().GetProperty("message")?.GetValue(notFoundResult.Value));
        }

        // Test for GetOrdersSortedByTotalAmount action in OrderController returning BadRequest when service throws exception
        [Fact]
        public async Task GetOrdersSortedByTotalAmount_ReturnsBadRequest_WhenServiceThrowsException()
        {
            // Arrange
            var orderServiceMock = new Mock<IOrderService>();
            var loggerMock = new Mock<ILogger<OrderController>>();
            var controller = new OrderController(orderServiceMock.Object, loggerMock.Object);

            orderServiceMock
                .Setup(s => s.GetOrdersSortedByTotalAmountAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Service error"));

            // Act
            var result = await controller.GetOrdersSortedByTotalAmount(CancellationToken.None);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Error has occurred while fetching sorted orders", badRequestResult.Value.ToString());
        }
    }



}



