using System.ComponentModel.DataAnnotations;
using DemoOrders.Domain.Enums;

namespace DemoOrders.Application.Dtos
{
    public class CreateOrderDto
    {
        [Required]
        [MaxLength(100)]
        public string CustomerName { get; set; }

        [Required]
        public OrderPaymentMethod PaymentMethod { get; set; }

        [Required]
        [MaxLength(200)]
        public string DeliveryAddress { get; set; }

        [Required]
        [Phone]
        [MaxLength(20)]
        public string ContactNumber { get; set; }

        [MaxLength(500)]
        public string? Note { get; set; }

        [Required]
        public OrderCurrency Currency { get; set; }

        [Required]
        public List<CreateOrderItemDto> Items { get; set; }
    }
}
