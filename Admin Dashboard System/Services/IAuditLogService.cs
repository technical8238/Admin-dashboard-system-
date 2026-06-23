using Admin_Dashboard_System.Models;

namespace Admin_Dashboard_System.Services
{
    public interface IAuditLogService
    {
        Task LogActionAsync(string userName, string action, string entityType, int? entityId = null, string? description = null, string? ipAddress = null);
        Task<IEnumerable<AuditLog>> GetAllLogsAsync();
        Task<IEnumerable<AuditLog>> GetByUserNameAsync(string userName);
        Task<IEnumerable<AuditLog>> GetByEntityTypeAsync(string entityType);
        Task<IEnumerable<AuditLog>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    }
}
