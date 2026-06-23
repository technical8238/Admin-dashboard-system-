using Admin_Dashboard_System.Models;
using Admin_Dashboard_System.Repositories;
using Admin_Dashboard_System.ViewModels;

namespace Admin_Dashboard_System.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryViewModel>> GetAllCategoriesAsync();
        Task<CategoryViewModel?> GetCategoryByIdAsync(int id);
        Task CreateCategoryAsync(CreateCategoryViewModel model);
        Task UpdateCategoryAsync(EditCategoryViewModel model);
        Task DeleteCategoryAsync(int id);
        Task<int> GetTotalCategoriesCountAsync();
    }

    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IEnumerable<CategoryViewModel>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return categories.Select(c => new CategoryViewModel
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                IsActive = c.IsActive,
                CreatedDate = c.CreatedDate,
                ProductCount = c.Products.Count
            });
        }

        public async Task<CategoryViewModel?> GetCategoryByIdAsync(int id)
        {
            var category = await _categoryRepository.GetByIdWithProductsAsync(id);
            if (category == null) return null;

            return new CategoryViewModel
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                IsActive = category.IsActive,
                CreatedDate = category.CreatedDate,
                ProductCount = category.Products.Count
            };
        }

        public async Task CreateCategoryAsync(CreateCategoryViewModel model)
        {
            var category = new Category
            {
                Name = model.Name,
                Description = model.Description
            };

            await _categoryRepository.AddAsync(category);
        }

        public async Task UpdateCategoryAsync(EditCategoryViewModel model)
        {
            var category = await _categoryRepository.GetByIdAsync(model.Id);
            if (category == null) throw new Exception("Category not found");

            category.Name = model.Name;
            category.Description = model.Description;
            category.IsActive = model.IsActive;

            await _categoryRepository.UpdateAsync(category);
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null) throw new Exception("Category not found");

            await _categoryRepository.DeleteAsync(category);
        }

        public async Task<int> GetTotalCategoriesCountAsync()
        {
            return await _categoryRepository.CountAsync();
        }
    }
}