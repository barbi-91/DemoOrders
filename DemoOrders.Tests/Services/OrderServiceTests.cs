using AutoMapper;
using DemoOrders.Application.Abstration.Repositories;
using DemoOrders.Application.Dtos;
using DemoOrders.Application.Mapping;
using DemoOrders.Application.Services;
using DemoOrders.Domain.Entities;
using DemoOrders.Domain.Enums;
using Microsoft.Extensions.Logging;
using Moq;

namespace DemoOrders.Tests.Services
{
    public class OrderServiceTests
    {
        // Test for CreateOrderAsync for successful order creation with mocked dependencies 
        [Fact]
        public async Task CreateOrderAsync_ReturnOrderDto()
        {
            // Arrange
            // 1.Mock repository instead of using a real database repository
            var orderRepositoryMock = new Mock<IOrderRepository>();

            // 2. Mock logger - no need to verify logging in this test
            var loggerMock = new Mock<ILogger<OrderService>>();
            // 3. AutoMapper -  use real AutoMapper with the actual mapping profile
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            var mapper = config.CreateMapper();
            // 4. Prepare expected Order object to be returned by the repository mock
            var createOrderDto = new CreateOrderDto
            {
                CustomerName = "John Doe",
                PaymentMethod = OrderPaymentMethod.Cash,
                DeliveryAddress = "123 Main St",
                ContactNumber = "023-456-7890",
                Note = "Leave at the front door",
                Currency = OrderCurrency.USD,
                Items = new List<CreateOrderItemDto>
                {
                    new CreateOrderItemDto
                    {
                        ProductName = "Product 1",
                        Quantity = 2,
                        Price = 10.5m
                    }
                }
            };

            // 5. Prepare the Order object that the repository mock will return
            var expectedOrder = new Order
            {
                Id = 1,
                CustomerName = createOrderDto.CustomerName,
                Status = OrderStatus.Pending,
                OrderTime = DateTime.UtcNow,
                PaymentMethod = createOrderDto.PaymentMethod,
                DeliveryAddress = createOrderDto.DeliveryAddress,
                ContactNumber = createOrderDto.ContactNumber,
                Note = createOrderDto.Note,
                Currency = createOrderDto.Currency,
                Items = new List<OrderItem>
                {
                    new OrderItem
                    {
                        ProductName = "Product 1",
                        Quantity = 2,
                        Price = 10.5m
                    }
                }
            };
            // 6. Setup the repository mock to return the expected Order when CreateOrderAsync is called
            orderRepositoryMock
                .Setup(repo => repo.CreateOrderAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedOrder);

            // 7. Create service instance with mocked dependencies
            var orderService = new OrderService(orderRepositoryMock.Object, mapper, loggerMock.Object);


            // Act
            var result = await orderService.CreateOrderAsync(createOrderDto, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedOrder.CustomerName, result.CustomerName);
            Assert.Single(result.Items);
            Assert.Equal(expectedOrder.Items[0].ProductName, result.Items[0].ProductName);
        }



        // Test for CreateOrderAsync to verify exception is thrown when creating an order without items
        [Fact]
        public async Task CreateOrderAsync_WithoutItems_ThrowsException()
        {
            // Arrange
            // Mock repository
            var orderRepositoryMock = new Mock<IOrderRepository>();
            // Mock logger
            var loggerMock = new Mock<ILogger<OrderService>>();
            // AutoMapper with actual mapping profile
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            var mapper = config.CreateMapper();

            // CreateOrderDto without items
            var createOrderDto = new CreateOrderDto
            {
                CustomerName = "Test User",
                PaymentMethod = OrderPaymentMethod.Cash,
                DeliveryAddress = "Test Address",
                ContactNumber = "123456789",
                Note = "",
                Currency = OrderCurrency.EUR,
                Items = new List<CreateOrderItemDto>() // Empty list
            };
            // Create service instance
            var orderService = new OrderService(orderRepositoryMock.Object, mapper, loggerMock.Object);

            // Act & Assert
            // Verify that an exception is thrown when creating an order without items
            await Assert.ThrowsAsync<Exception>(() =>
                orderService.CreateOrderAsync(createOrderDto, CancellationToken.None));
        }


    }
}
