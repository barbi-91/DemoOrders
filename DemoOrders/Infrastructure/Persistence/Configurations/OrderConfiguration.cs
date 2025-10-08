using DemoOrders.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DemoOrders.Infrastructure.Persistence.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            // Table name
            builder.ToTable("Orders");

            // Primary key
            builder.HasKey(o => o.Id);

            // Properties
            builder.Property(o => o.CustomerName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(o => o.Status)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(o => o.OrderTime)
                .IsRequired()
                .HasColumnType("datetime")
                .HasDefaultValueSql("GETDATE()"); // Default to current date/time in SQL Server

            builder.Property(o => o.PaymentMethod)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(o => o.DeliveryAddress)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(o => o.ContactNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(o => o.Note)
                .HasMaxLength(500);

            builder.Property(o => o.Currency)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(3);

            // Relationship with OrderItem: Order has many OrderItems
            builder.HasMany(o => o.Items)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Ignore TotalAmount property for EF Core and mapping
            builder.Ignore(o => o.TotalAmount);
        }
    }
}
