using Admin_Dashboard_System.Models;
using Admin_Dashboard_System.Repositories;
using Admin_Dashboard_System.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace Admin_Dashboard_System.Services
{
    public interface IDashboardService
    {
        Task<DashboardViewModel> GetDashboardDataAsync();
    }

    public class DashboardService : IDashboardService
    {
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IUserRepository _userRepository;

        public DashboardService(IProductRepository productRepository, IOrderRepository orderRepository, IUserRepository userRepository)
        {
            _productRepository = productRepository;
            _orderRepository = orderRepository;
            _userRepository = userRepository;
        }

        public async Task<DashboardViewModel> GetDashboardDataAsync()
        {
            var products = await _productRepository.GetAllAsync();
            var orders = await _orderRepository.GetAllAsync();
            var users = await _userRepository.GetAllAsync();

            return new DashboardViewModel
            {
                TotalUsers = users.Count(),
                TotalProducts = products.Count(),
                TotalOrders = orders.Count(),
                TotalRevenue = orders.Sum(o => o.TotalAmount),
                RecentActivities = GetRecentActivities(orders),
                SalesData = GetSalesData(orders),
                OrdersData = GetOrdersData(orders),
                TopProducts = GetTopProducts(orders, products),
                CategorySales = GetCategorySales(products),
                ActiveUsers = users.Count(u => u.IsActive),
                LowStockProducts = products.Count(p => p.StockQuantity < 10)
            };
        }

        private List<RecentActivityViewModel> GetRecentActivities(IEnumerable<Order> orders)
        {
            return orders.OrderByDescending(o => o.OrderDate)
                .Take(5)
                .Select(o => new RecentActivityViewModel
                {
                    Action = "New Order",
                    Description = $"Order #{o.Id} placed by {o.User?.Email ?? "Unknown"}",
                    Timestamp = o.OrderDate
                }).ToList();
        }

        private ChartDataViewModel GetSalesData(IEnumerable<Order> orders)
        {
            var salesByMonth = orders
                .Where(o => o.OrderDate >= DateTime.UtcNow.AddMonths(-6))
                .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Total = g.Sum(o => o.TotalAmount)
                })
                .OrderBy(g => g.Year)
                .ThenBy(g => g.Month)
                .ToList();

            return new ChartDataViewModel
            {
                Labels = salesByMonth.Select(s => $"{s.Year}-{s.Month:D2}").ToList(),
                Data = salesByMonth.Select(s => s.Total).ToList()
            };
        }

        private ChartDataViewModel GetOrdersData(IEnumerable<Order> orders)
        {
            var ordersByMonth = orders
                .Where(o => o.OrderDate >= DateTime.UtcNow.AddMonths(-6))
                .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Count = g.Count()
                })
                .OrderBy(g => g.Year)
                .ThenBy(g => g.Month)
                .ToList();

            return new ChartDataViewModel
            {
                Labels = ordersByMonth.Select(s => $"{s.Year}-{s.Month:D2}").ToList(),
                Data = ordersByMonth.Select(s => (decimal)s.Count).ToList()
            };
        }

        private List<ProductSalesViewModel> GetTopProducts(IEnumerable<Order> orders, IEnumerable<Product> products)
        {
            var productSales = orders
                .SelectMany(o => o.OrderItems)
                .GroupBy(oi => oi.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    TotalSales = g.Sum(oi => oi.Quantity),
                    TotalRevenue = g.Sum(oi => (oi.Product?.Price ?? 0) * oi.Quantity)
                })
                .OrderByDescending(p => p.TotalRevenue)
                .Take(10)
                .ToList();

            return productSales.Select(ps => new ProductSalesViewModel
            {
                ProductId = ps.ProductId,
                ProductName = products.FirstOrDefault(p => p.Id == ps.ProductId)?.Name ?? "Unknown",
                TotalSales = ps.TotalSales,
                TotalRevenue = ps.TotalRevenue
            }).ToList();
        }

        private List<CategorySalesViewModel> GetCategorySales(IEnumerable<Product> products)
        {
            return products
                .GroupBy(p => p.CategoryId)
                .Select(g => new CategorySalesViewModel
                {
                    CategoryId = g.Key,
                    CategoryName = g.FirstOrDefault()?.Category?.Name ?? "Unknown",
                    TotalProducts = g.Count(),
                    TotalRevenue = g.Sum(p => p.Price * p.StockQuantity)
                })
                .OrderByDescending(c => c.TotalRevenue)
                .ToList();
        }
    }
}