using DemoOrders.Application.Abstration.Repositories;
using DemoOrders.Domain.Entities;
using DemoOrders.Infrastructure.Persistence.AppDbContext;


public class OrderRepository : IOrderRepository
{
    private readonly ApplicationDbContext _context;

    public OrderRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    // Create a new order asynchronously and return the created order with its generated ID
    public async Task<Order> CreateOrderAsync(Order order, CancellationToken cancellationToken)
    {
        // Add the order to the context and save changes to generate the ID
        _context.Orders.Add(order);
        await _context.SaveChangesAsync(cancellationToken);

        // Return the created order entity with the generated ID
        return order;
    }
}

