using System.ComponentModel.DataAnnotations;
using DemoOrders.Domain.Enums;

namespace DemoOrders.Application.Dtos
{
    public class OrderDto
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string CustomerName { get; set; }

        [Required]
        public OrderStatus Status { get; set; }

        [Required]
        public DateTime OrderTime { get; set; }

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


        public List<OrderItemDto> Items { get; set; }

    }
}
