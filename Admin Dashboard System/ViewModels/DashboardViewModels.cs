namespace Admin_Dashboard_System.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalProducts { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public List<RecentActivityViewModel> RecentActivities { get; set; } = new();
        public ChartDataViewModel SalesData { get; set; } = new();
        public ChartDataViewModel OrdersData { get; set; } = new();
        public List<ProductSalesViewModel> TopProducts { get; set; } = new();
        public List<CategorySalesViewModel> CategorySales { get; set; } = new();
        public int ActiveUsers { get; set; }
        public int LowStockProducts { get; set; }
    }

    public class RecentActivityViewModel
    {
        public string Action { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }

    public class ChartDataViewModel
    {
        public List<string> Labels { get; set; } = new();
        public List<decimal> Data { get; set; } = new();
    }

    public class ProductSalesViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int TotalSales { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class CategorySalesViewModel
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int TotalProducts { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}