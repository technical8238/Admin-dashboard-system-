using Admin_Dashboard_System.Models;
using Admin_Dashboard_System.Repositories;
using Admin_Dashboard_System.ViewModels;

namespace Admin_Dashboard_System.Services
{
    public interface IShoppingCartService
    {
        Task<IEnumerable<ShoppingCartItemViewModel>> GetCartItemsAsync(string userId);
        Task AddToCartAsync(string userId, int productId, int quantity);
        Task UpdateQuantityAsync(string userId, int productId, int quantity);
        Task RemoveFromCartAsync(string userId, int productId);
        Task ClearCartAsync(string userId);
        Task<decimal> GetCartTotalAsync(string userId);
    }

    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IShoppingCartRepository _cartRepository;
        private readonly IProductRepository _productRepository;

        public ShoppingCartService(IShoppingCartRepository cartRepository, IProductRepository productRepository)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<ShoppingCartItemViewModel>> GetCartItemsAsync(string userId)
        {
            var cartItems = await _cartRepository.GetByUserIdAsync(userId);
            return cartItems.Select(item => new ShoppingCartItemViewModel
            {
                CartItemId = item.Id,
                ProductId = item.ProductId,
                ProductName = item.Product?.Name ?? "Unknown",
                ProductDescription = item.Product?.Description ?? "",
                Price = item.Product?.Price ?? 0,
                ImageUrl = item.Product?.ImageUrl,
                Quantity = item.Quantity,
                Subtotal = (item.Product?.Price ?? 0) * item.Quantity
            }).ToList();
        }

        public async Task AddToCartAsync(string userId, int productId, int quantity)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null || !product.IsActive)
                throw new Exception("Product not found or not available");

            if (product.StockQuantity < quantity)
                throw new Exception("Insufficient stock");

            var existingItem = await _cartRepository.GetByUserAndProductAsync(userId, productId);
            
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
                if (existingItem.Quantity > product.StockQuantity)
                    throw new Exception("Insufficient stock");
                await _cartRepository.UpdateAsync(existingItem);
            }
            else
            {
                var cartItem = new ShoppingCart
                {
                    UserId = userId,
                    ProductId = productId,
                    Quantity = quantity
                };
                await _cartRepository.AddAsync(cartItem);
            }
        }

        public async Task UpdateQuantityAsync(string userId, int productId, int quantity)
        {
            if (quantity < 1)
                throw new Exception("Quantity must be at least 1");

            var cartItem = await _cartRepository.GetByUserAndProductAsync(userId, productId);
            if (cartItem == null)
                throw new Exception("Item not found in cart");

            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null || product.StockQuantity < quantity)
                throw new Exception("Insufficient stock");

            cartItem.Quantity = quantity;
            await _cartRepository.UpdateAsync(cartItem);
        }

        public async Task RemoveFromCartAsync(string userId, int productId)
        {
            var cartItem = await _cartRepository.GetByUserAndProductAsync(userId, productId);
            if (cartItem != null)
            {
                await _cartRepository.DeleteAsync(cartItem);
            }
        }

        public async Task ClearCartAsync(string userId)
        {
            await _cartRepository.ClearCartAsync(userId);
        }

        public async Task<decimal> GetCartTotalAsync(string userId)
        {
            var cartItems = await _cartRepository.GetByUserIdAsync(userId);
            return cartItems.Sum(item => (item.Product?.Price ?? 0) * item.Quantity);
        }
    }
}
