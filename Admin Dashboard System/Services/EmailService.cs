using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;

namespace Admin_Dashboard_System.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true)
        {
            try
            {
                var emailSettings = _configuration.GetSection("EmailSettings");
                var fromEmail = emailSettings["FromEmail"] ?? "noreply@admindashboard.com";
                var fromName = emailSettings["FromName"] ?? "Admin Dashboard";
                var smtpServer = emailSettings["SmtpServer"] ?? "smtp.gmail.com";
                var smtpPort = int.Parse(emailSettings["SmtpPort"] ?? "587");
                var smtpUsername = emailSettings["SmtpUsername"] ?? fromEmail;
                var smtpPassword = emailSettings["SmtpPassword"] ?? "";
                var useSsl = bool.Parse(emailSettings["UseSsl"] ?? "true");

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(fromName, fromEmail));
                message.To.Add(new MailboxAddress("", toEmail));
                message.Subject = subject;

                message.Body = new TextPart(isHtml ? "html" : "plain")
                {
                    Text = body
                };

                using var client = new SmtpClient();
                await client.ConnectAsync(smtpServer, smtpPort, useSsl);
                await client.AuthenticateAsync(smtpUsername, smtpPassword);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation($"Email sent successfully to {toEmail}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email to {toEmail}");
                throw;
            }
        }

        public async Task SendWelcomeEmailAsync(string toEmail, string userName)
        {
            var subject = "Welcome to Admin Dashboard";
            var body = $@"
                <html>
                <body>
                    <h2>Welcome to Admin Dashboard, {userName}!</h2>
                    <p>Thank you for registering with us. Your account has been successfully created.</p>
                    <p>You can now log in and start managing your products, orders, and more.</p>
                    <p>If you have any questions, please don't hesitate to contact our support team.</p>
                    <p>Best regards,<br>Admin Dashboard Team</p>
                </body>
                </html>";

            await SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendOrderConfirmationAsync(string toEmail, string userName, int orderId, decimal totalAmount)
        {
            var subject = $"Order Confirmation - Order #{orderId}";
            var body = $@"
                <html>
                <body>
                    <h2>Order Confirmation</h2>
                    <p>Dear {userName},</p>
                    <p>Your order <strong>#{orderId}</strong> has been successfully placed.</p>
                    <p><strong>Total Amount:</strong> ${totalAmount:F2}</p>
                    <p>Thank you for your purchase!</p>
                    <p>Best regards,<br>Admin Dashboard Team</p>
                </body>
                </html>";

            await SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendLowStockAlertAsync(string toEmail, string productName, int stockQuantity)
        {
            var subject = $"Low Stock Alert - {productName}";
            var body = $@"
                <html>
                <body>
                    <h2>Low Stock Alert</h2>
                    <p>The product <strong>{productName}</strong> is running low on stock.</p>
                    <p><strong>Current Stock:</strong> {stockQuantity} units</p>
                    <p>Please restock this item as soon as possible.</p>
                    <p>Best regards,<br>Admin Dashboard Team</p>
                </body>
                </html>";

            await SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendPasswordResetEmailAsync(string toEmail, string resetLink)
        {
            var subject = "Password Reset Request";
            var body = $@"
                <html>
                <body>
                    <h2>Password Reset Request</h2>
                    <p>You requested a password reset for your account.</p>
                    <p>Click the link below to reset your password:</p>
                    <p><a href='{resetLink}'>Reset Password</a></p>
                    <p>This link will expire in 24 hours.</p>
                    <p>If you did not request this password reset, please ignore this email.</p>
                    <p>Best regards,<br>Admin Dashboard Team</p>
                </body>
                </html>";

            await SendEmailAsync(toEmail, subject, body);
        }
    }
}
