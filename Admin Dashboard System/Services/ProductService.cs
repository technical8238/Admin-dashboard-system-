using Admin_Dashboard_System.Models;
using Admin_Dashboard_System.Repositories;
using Admin_Dashboard_System.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

namespace Admin_Dashboard_System.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductViewModel>> GetAllProductsAsync();
        Task<ProductViewModel?> GetProductByIdAsync(int id);
        Task<CreateProductViewModel> GetCreateProductViewModelAsync();
        Task<EditProductViewModel> GetEditProductViewModelAsync(int id);
        Task CreateProductAsync(CreateProductViewModel model);
        Task UpdateProductAsync(EditProductViewModel model);
        Task DeleteProductAsync(int id);
        Task<int> GetTotalProductsCountAsync();
        Task<IEnumerable<ProductViewModel>> SearchProductsAsync(string searchTerm);
        Task ClearCacheAsync();
    }

    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMemoryCache _cache;
        private readonly string _cacheKeyPrefix = "Products_";

        public ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository, IWebHostEnvironment webHostEnvironment, IMemoryCache cache)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _webHostEnvironment = webHostEnvironment;
            _cache = cache;
        }

        private async Task<string> SaveImageFile(IFormFile? imageFile)
        {
            if (imageFile == null || imageFile.Length == 0) return null;

            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            return "/images/products/" + uniqueFileName;
        }

        private void DeleteImageFile(string? imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl)) return;

            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, imageUrl.TrimStart('/'));
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        public async Task<IEnumerable<ProductViewModel>> GetAllProductsAsync()
        {
            string cacheKey = _cacheKeyPrefix + "All";
            
            if (!_cache.TryGetValue(cacheKey, out IEnumerable<ProductViewModel>? cachedProducts))
            {
                var products = await _productRepository.GetAllWithCategoryAsync();
                cachedProducts = products.Select(p => new ProductViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    CategoryName = p.Category?.Name ?? "Unknown",
                    ImageUrl = p.ImageUrl,
                    IsActive = p.IsActive,
                    CreatedDate = p.CreatedDate
                }).ToList();
                
                _cache.Set(cacheKey, cachedProducts, TimeSpan.FromMinutes(5));
            }
            
            return cachedProducts!;
        }

        public async Task<ProductViewModel?> GetProductByIdAsync(int id)
        {
            var product = await _productRepository.GetByIdWithCategoryAsync(id);
            if (product == null) return null;

            return new ProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                CategoryName = product.Category.Name,
                ImageUrl = product.ImageUrl,
                IsActive = product.IsActive,
                CreatedDate = product.CreatedDate
            };
        }

        public async Task<CreateProductViewModel> GetCreateProductViewModelAsync()
        {
            var categories = await _categoryRepository.GetActiveCategoriesAsync();
            return new CreateProductViewModel
            {
                Categories = categories.Select(c => new CategoryViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description
                }).ToList()
            };
        }

        public async Task<EditProductViewModel> GetEditProductViewModelAsync(int id)
        {
            var product = await _productRepository.GetByIdWithCategoryAsync(id);
            if (product == null) throw new Exception("Product not found");

            var categories = await _categoryRepository.GetActiveCategoriesAsync();

            return new EditProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                CategoryId = product.CategoryId,
                IsActive = product.IsActive,
                ExistingImageUrl = product.ImageUrl,
                Categories = categories.Select(c => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name,
                    Selected = c.Id == product.CategoryId
                })
            };
        }

        public async Task CreateProductAsync(CreateProductViewModel model)
        {
            var imageUrl = await SaveImageFile(model.ImageFile);

            var product = new Product
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                StockQuantity = model.StockQuantity,
                CategoryId = model.CategoryId,
                ImageUrl = imageUrl
            };

            await _productRepository.AddAsync(product);
            await ClearCacheAsync();
        }

        public async Task UpdateProductAsync(EditProductViewModel model)
        {
            var product = await _productRepository.GetByIdAsync(model.Id);
            if (product == null) throw new Exception("Product not found");

            product.Name = model.Name;
            product.Description = model.Description;
            product.Price = model.Price;
            product.StockQuantity = model.StockQuantity;
            product.CategoryId = model.CategoryId;
            product.IsActive = model.IsActive;

            if (model.ImageFile != null)
            {
                DeleteImageFile(product.ImageUrl);
                product.ImageUrl = await SaveImageFile(model.ImageFile);
            }

            await _productRepository.UpdateAsync(product);
            await ClearCacheAsync();
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) throw new Exception("Product not found");

            DeleteImageFile(product.ImageUrl);
            await _productRepository.DeleteAsync(product);
            await ClearCacheAsync();
        }

        public async Task<int> GetTotalProductsCountAsync()
        {
            return await _productRepository.CountAsync();
        }

        public async Task<IEnumerable<ProductViewModel>> SearchProductsAsync(string searchTerm)
        {
            var products = await _productRepository.SearchProductsAsync(searchTerm);
            return products.Select(p => new ProductViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                CategoryName = p.Category.Name,
                ImageUrl = p.ImageUrl,
                IsActive = p.IsActive,
                CreatedDate = p.CreatedDate
            });
        }

        public async Task ClearCacheAsync()
        {
            _cache.Remove(_cacheKeyPrefix + "All");
        }
    }
}