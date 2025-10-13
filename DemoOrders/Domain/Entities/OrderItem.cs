using System.ComponentModel.DataAnnotations;

namespace DemoOrders.Domain.Entities
{
    public class OrderItem
    {
        // Primary key
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string ProductName { get; set; } = string.Empty;

        [Required]
        [Range(1, 10000, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }

        [Required]
        [Range(0, 1000000, ErrorMessage = "Price must be non-negative")]
        public decimal Price { get; set; }

        // Foreign key to the Order
        [Required]
        public int OrderId { get; set; }

        // Navigation property
        public Order? Order { get; set; }
    }
}
