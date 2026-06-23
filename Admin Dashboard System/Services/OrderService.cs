using Admin_Dashboard_System.Models;
using Admin_Dashboard_System.Repositories;
using Admin_Dashboard_System.ViewModels;

namespace Admin_Dashboard_System.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderViewModel>> GetAllOrdersAsync();
        Task<OrderDetailsViewModel?> GetOrderDetailsAsync(int id);
        Task CreateOrderAsync(CreateOrderViewModel model);
        Task UpdateOrderStatusAsync(int id, string status);
        Task DeleteOrderAsync(int id);
        Task<int> GetTotalOrdersCountAsync();
        Task<decimal> GetTotalRevenueAsync();
    }

    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUserRepository _userRepository;
        private readonly IProductRepository _productRepository;

        public OrderService(IOrderRepository orderRepository, IUserRepository userRepository, IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _userRepository = userRepository;
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<OrderViewModel>> GetAllOrdersAsync()
        {
            var orders = await _orderRepository.GetAllAsync();
            return orders.Select(o => new OrderViewModel
            {
                Id = o.Id,
                UserName = o.User != null ? $"{o.User.FirstName} {o.User.LastName}" : "Unknown User",
                UserEmail = o.User?.Email ?? "",
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                Status = o.Status,
                ItemCount = o.OrderItems != null ? o.OrderItems.Sum(oi => oi.Quantity) : 0
            });
        }

        public async Task<OrderDetailsViewModel?> GetOrderDetailsAsync(int id)
        {
            var order = await _orderRepository.GetByIdWithDetailsAsync(id);
            if (order == null) return null;

            return new OrderDetailsViewModel
            {
                Id = order.Id,
                UserName = $"{order.User.FirstName} {order.User.LastName}",
                UserEmail = order.User.Email ?? "",
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                OrderItems = order.OrderItems.Select(oi => new OrderItemViewModel
                {
                    Id = oi.Id,
                    ProductName = oi.Product.Name,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList()
            };
        }

        public async Task CreateOrderAsync(CreateOrderViewModel model)
        {
            var user = await _userRepository.GetByIdAsync(model.UserId);
            if (user == null) throw new Exception("User not found");

            decimal totalAmount = 0;
            var orderItems = new List<OrderItem>();

            foreach (var item in model.OrderItems)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);
                if (product == null) throw new Exception($"Product {item.ProductId} not found");

                if (product.StockQuantity < item.Quantity)
                    throw new Exception($"Insufficient stock for product {product.Name}");

                totalAmount += product.Price * item.Quantity;

                orderItems.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price
                });

                // Update stock
                product.StockQuantity -= item.Quantity;
                await _productRepository.UpdateAsync(product);
            }

            var order = new Order
            {
                UserId = model.UserId,
                TotalAmount = totalAmount,
                PaymentMethod = model.PaymentMethod,
                ShippingAddress = model.ShippingAddress,
                Notes = model.Notes,
                OrderItems = orderItems
            };

            await _orderRepository.AddAsync(order);
        }

        public async Task UpdateOrderStatusAsync(int id, string status)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null) throw new Exception("Order not found");

            order.Status = status;
            await _orderRepository.UpdateAsync(order);
        }

        public async Task DeleteOrderAsync(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null) throw new Exception("Order not found");

            await _orderRepository.DeleteAsync(order);
        }

        public async Task<int> GetTotalOrdersCountAsync()
        {
            return await _orderRepository.CountAsync();
        }

        public async Task<decimal> GetTotalRevenueAsync()
        {
            return await _orderRepository.GetTotalRevenueAsync();
        }
    }
}