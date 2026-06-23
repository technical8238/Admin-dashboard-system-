namespace Admin_Dashboard_System.ViewModels
{
    public class OrderViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public int ItemCount { get; set; }
    }

    public class OrderDetailsViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<OrderItemViewModel> OrderItems { get; set; } = new();
    }

    public class OrderItemViewModel
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice => Quantity * UnitPrice;
    }

    public class CreateOrderViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = "CashOnDelivery";
        public string ShippingAddress { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public List<OrderItemCreateViewModel> OrderItems { get; set; } = new();
    }

    public class OrderItemCreateViewModel
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class ShoppingCartItemViewModel
    {
        public int CartItemId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductDescription { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal { get; set; }
    }

    public class ShoppingCartViewModel
    {
        public List<ShoppingCartItemViewModel> Items { get; set; } = new();
        public decimal TotalAmount { get; set; }
        public int TotalItems { get; set; }
    }

    public class CheckoutViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public List<ShoppingCartItemViewModel> CartItems { get; set; } = new();
        public decimal TotalAmount { get; set; }
        public string ShippingAddress { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = "CashOnDelivery";
        public string? Notes { get; set; }
    }
}