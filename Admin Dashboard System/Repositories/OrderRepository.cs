using Admin_Dashboard_System.Data;
using Admin_Dashboard_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Admin_Dashboard_System.Repositories
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<Order?> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<Order>> GetOrdersByUserAsync(string userId);
        Task<IEnumerable<Order>> GetOrdersByStatusAsync(string status);
        Task<decimal> GetTotalRevenueAsync();
    }

    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Order?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserAsync(string userId)
        {
            return await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersByStatusAsync(string status)
        {
            return await _context.Orders
                .Where(o => o.Status == status)
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalRevenueAsync()
        {
            return await _context.Orders
                .Where(o => o.Status != "Cancelled")
                .SumAsync(o => o.TotalAmount);
        }
    }
}