using System.ComponentModel.DataAnnotations.Schema;

namespace Admin_Dashboard_System.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, Processing, Shipped, Delivered, Cancelled
        public string? PaymentMethod { get; set; } // CashOnDelivery, Card, UPI, BankTransfer
        public string? ShippingAddress { get; set; }
        public string? Notes { get; set; }

        // Navigation
        public ApplicationUser User { get; set; } = null!;
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }

    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        // Navigation
        public Order Order { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}