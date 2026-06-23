using Microsoft.AspNetCore.SignalR;

namespace Admin_Dashboard_System.Hubs
{
    public class DashboardHub : Hub
    {
        public async Task NotifyOrderCreated(int orderId, string customerEmail, decimal totalAmount)
        {
            await Clients.All.SendAsync("OrderCreated", new { orderId, customerEmail, totalAmount, timestamp = DateTime.UtcNow });
        }

        public async Task NotifyProductUpdated(int productId, string productName)
        {
            await Clients.All.SendAsync("ProductUpdated", new { productId, productName, timestamp = DateTime.UtcNow });
        }

        public async Task NotifyUserRegistered(string userEmail)
        {
            await Clients.All.SendAsync("UserRegistered", new { userEmail, timestamp = DateTime.UtcNow });
        }

        public async Task NotifyLowStockAlert(string productName, int stockQuantity)
        {
            await Clients.All.SendAsync("LowStockAlert", new { productName, stockQuantity, timestamp = DateTime.UtcNow });
        }

        public async Task NotifyDashboardUpdate()
        {
            await Clients.All.SendAsync("DashboardUpdate", new { timestamp = DateTime.UtcNow });
        }
    }
}
