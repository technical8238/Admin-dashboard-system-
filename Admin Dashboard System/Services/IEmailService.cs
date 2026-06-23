namespace Admin_Dashboard_System.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true);
        Task SendWelcomeEmailAsync(string toEmail, string userName);
        Task SendOrderConfirmationAsync(string toEmail, string userName, int orderId, decimal totalAmount);
        Task SendLowStockAlertAsync(string toEmail, string productName, int stockQuantity);
        Task SendPasswordResetEmailAsync(string toEmail, string resetLink);
    }
}
