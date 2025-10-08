using DemoOrders.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DemoOrders.Application.Abstractions
{
    public interface IApplicationDbContext
    {
        DbSet<Order> Orders { get; set; }
        DbSet<OrderItem> OrderItems { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
