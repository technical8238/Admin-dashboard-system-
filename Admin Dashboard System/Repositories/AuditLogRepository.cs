using Admin_Dashboard_System.Data;
using Admin_Dashboard_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Admin_Dashboard_System.Repositories
{
    public class AuditLogRepository : Repository<AuditLog>, IAuditLogRepository
    {
        private readonly ApplicationDbContext _context;

        public AuditLogRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AuditLog>> GetByUserNameAsync(string userName)
        {
            return await _context.AuditLogs
                .Where(a => a.UserName == userName)
                .OrderByDescending(a => a.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetByEntityTypeAsync(string entityType)
        {
            return await _context.AuditLogs
                .Where(a => a.EntityType == entityType)
                .OrderByDescending(a => a.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.AuditLogs
                .Where(a => a.CreatedDate >= startDate && a.CreatedDate <= endDate)
                .OrderByDescending(a => a.CreatedDate)
                .ToListAsync();
        }
    }
}
