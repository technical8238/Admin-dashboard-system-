using Admin_Dashboard_System.Models;

namespace Admin_Dashboard_System.Repositories
{
    public interface IAuditLogRepository : IRepository<AuditLog>
    {
        Task<IEnumerable<AuditLog>> GetByUserNameAsync(string userName);
        Task<IEnumerable<AuditLog>> GetByEntityTypeAsync(string entityType);
        Task<IEnumerable<AuditLog>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    }
}
