using Admin_Dashboard_System.Data;
using Admin_Dashboard_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Admin_Dashboard_System.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<Product?> GetByIdWithCategoryAsync(int id);
        Task<IEnumerable<Product>> GetActiveProductsAsync();
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId);
        Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm);
        Task<IEnumerable<Product>> GetAllWithCategoryAsync();
    }

    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Product?> GetByIdWithCategoryAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Product>> GetActiveProductsAsync()
        {
            return await _context.Products
                .Where(p => p.IsActive)
                .Include(p => p.Category)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            return await _context.Products
                .Where(p => p.CategoryId == categoryId && p.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm)
        {
            return await _context.Products
                .Where(p => p.IsActive && (p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm)))
                .Include(p => p.Category)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetAllWithCategoryAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .ToListAsync();
        }
    }
}