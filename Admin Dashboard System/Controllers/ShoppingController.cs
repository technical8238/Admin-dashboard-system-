using Admin_Dashboard_System.Services;
using Admin_Dashboard_System.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Admin_Dashboard_System.Controllers
{
    [Authorize]
    public class ShoppingController : Controller
    {
        private readonly IShoppingCartService _cartService;
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;

        public ShoppingController(IShoppingCartService cartService, IProductService productService, IOrderService orderService)
        {
            _cartService = cartService;
            _productService = productService;
            _orderService = orderService;
        }

        public async Task<IActionResult> Catalog()
        {
            var products = await _productService.GetAllProductsAsync();
            return View(products.Where(p => p.IsActive));
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? throw new Exception("User not authenticated");
            var cartItems = await _cartService.GetCartItemsAsync(userId);
            var total = await _cartService.GetCartTotalAsync(userId);

            var model = new ShoppingCartViewModel
            {
                Items = cartItems.ToList(),
                TotalAmount = total,
                TotalItems = cartItems.Sum(item => item.Quantity)
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? throw new Exception("User not authenticated");

            try
            {
                await _cartService.AddToCartAsync(userId, productId, quantity);
                TempData["Success"] = "Product added to cart";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuantity(int productId, int quantity)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? throw new Exception("User not authenticated");

            try
            {
                await _cartService.UpdateQuantityAsync(userId, productId, quantity);
                TempData["Success"] = "Cart updated";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFromCart(int productId)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? throw new Exception("User not authenticated");

            try
            {
                await _cartService.RemoveFromCartAsync(userId, productId);
                TempData["Success"] = "Item removed from cart";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Checkout()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? throw new Exception("User not authenticated");
            var cartItems = await _cartService.GetCartItemsAsync(userId);

            if (!cartItems.Any())
            {
                TempData["Error"] = "Your cart is empty";
                return RedirectToAction(nameof(Index));
            }

            var total = await _cartService.GetCartTotalAsync(userId);

            var model = new CheckoutViewModel
            {
                UserId = userId,
                UserName = User?.Identity?.Name ?? "",
                UserEmail = User?.Identity?.Name ?? "",
                CartItems = cartItems.ToList(),
                TotalAmount = total
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(CheckoutViewModel model)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? throw new Exception("User not authenticated");

            if (!ModelState.IsValid)
            {
                var cartItems = await _cartService.GetCartItemsAsync(userId);
                model.CartItems = cartItems.ToList();
                model.TotalAmount = await _cartService.GetCartTotalAsync(userId);
                return View("Checkout", model);
            }

            try
            {
                var cartItems = await _cartService.GetCartItemsAsync(userId);

                var createOrderModel = new CreateOrderViewModel
                {
                    UserId = userId,
                    PaymentMethod = model.PaymentMethod,
                    ShippingAddress = model.ShippingAddress,
                    Notes = model.Notes,
                    OrderItems = cartItems.Select(item => new OrderItemCreateViewModel
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity
                    }).ToList()
                };

                await _orderService.CreateOrderAsync(createOrderModel);

                // Clear the cart after successful order
                await _cartService.ClearCartAsync(userId);

                TempData["Success"] = "Order placed successfully!";
                return RedirectToAction("Index", "Orders");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Checkout");
            }
        }
    }
}
