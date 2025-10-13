using System.ComponentModel.DataAnnotations;

namespace DemoOrders.Application.Dtos
{
    public class OrderItemDto
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string ProductName { get; set; } = null!;

        [Required]
        [Range(1, 10000, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }

        [Required]
        [Range(0, 1000000, ErrorMessage = "Price must be non-negative")]
        public decimal Price { get; set; }
    }
}
