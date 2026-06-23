using Admin_Dashboard_System.Data;
using Admin_Dashboard_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Admin_Dashboard_System.Repositories
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<Category?> GetByIdWithProductsAsync(int id);
        Task<IEnumerable<Category>> GetActiveCategoriesAsync();
    }

    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Category?> GetByIdWithProductsAsync(int id)
        {
            return await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Category>> GetActiveCategoriesAsync()
        {
            return await _context.Categories
                .Where(c => c.IsActive)
                .ToListAsync();
        }
    }
}