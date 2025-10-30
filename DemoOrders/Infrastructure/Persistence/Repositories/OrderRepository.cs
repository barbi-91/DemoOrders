using DemoOrders.Application.Abstration.Repositories;
using DemoOrders.Domain.Entities;
using DemoOrders.Domain.Enums;
using DemoOrders.Infrastructure.Persistence.AppDbContext;
using Microsoft.EntityFrameworkCore;


public class OrderRepository : IOrderRepository
{
    private readonly ApplicationDbContext _context;

    public OrderRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    // Create a new orderById asynchronously and return the created orderById with its generated ID
    public async Task<Order> CreateOrderAsync(Order order, CancellationToken cancellationToken)
    {
        // Add the orderById to the context and save changes to generate the ID
        _context.Orders.Add(order);
        await _context.SaveChangesAsync(cancellationToken);

        // Return the created orderById entity with the generated ID
        return order;
    }

    // Get all orders asynchronously and return a IEnumerable of orders
    public async Task<IEnumerable<Order>> GetAllOrdersAsync(CancellationToken cancellationToken)
    {
        return await _context.Orders
            .Include(o => o.Items)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }



    public async Task<Order?> UpdateOrderStatusAsync(int orderId, OrderStatus status, CancellationToken cancellationToken)
    {
        Order? orderById = await GetOrderByIdAsync(orderId);
        if (orderById == null)
        {
            return null; // Order not found
        }
        // Update the status
        orderById.Status = status;
        // Save changes
        await _context.SaveChangesAsync(cancellationToken);
        return orderById;
    }

    // Helper method get single method by Id
    public async Task<Order?> GetOrderByIdAsync(int orderId)
    {
        Order? orderById = await _context.Orders
            .Include(o => o.Items)
            .AsNoTracking()
            .SingleOrDefaultAsync(o => o.Id == orderId); // return the orderById or null if not found
        return orderById;
    }
}

