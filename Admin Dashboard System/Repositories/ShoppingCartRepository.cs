using Admin_Dashboard_System.Data;
using Admin_Dashboard_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Admin_Dashboard_System.Repositories
{
    public interface IShoppingCartRepository : IRepository<ShoppingCart>
    {
        Task<IEnumerable<ShoppingCart>> GetByUserIdAsync(string userId);
        Task<ShoppingCart?> GetByUserAndProductAsync(string userId, int productId);
        Task ClearCartAsync(string userId);
    }

    public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
    {
        public ShoppingCartRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ShoppingCart>> GetByUserIdAsync(string userId)
        {
            return await _context.ShoppingCarts
                .Include(sc => sc.Product)
                .Where(sc => sc.UserId == userId)
                .ToListAsync();
        }

        public async Task<ShoppingCart?> GetByUserAndProductAsync(string userId, int productId)
        {
            return await _context.ShoppingCarts
                .FirstOrDefaultAsync(sc => sc.UserId == userId && sc.ProductId == productId);
        }

        public async Task ClearCartAsync(string userId)
        {
            var cartItems = await _context.ShoppingCarts
                .Where(sc => sc.UserId == userId)
                .ToListAsync();
            
            _context.ShoppingCarts.RemoveRange(cartItems);
            await _context.SaveChangesAsync();
        }
    }
}
