using Admin_Dashboard_System.Models;
using Admin_Dashboard_System.Repositories;

namespace Admin_Dashboard_System.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IAuditLogRepository _auditLogRepository;

        public AuditLogService(IAuditLogRepository auditLogRepository)
        {
            _auditLogRepository = auditLogRepository;
        }

        public async Task LogActionAsync(string userName, string action, string entityType, int? entityId = null, string? description = null, string? ipAddress = null)
        {
            var auditLog = new AuditLog
            {
                UserName = userName,
                Action = action,
                EntityType = entityType,
                EntityId = entityId,
                Description = description,
                IpAddress = ipAddress,
                CreatedDate = DateTime.UtcNow
            };

            await _auditLogRepository.AddAsync(auditLog);
        }

        public async Task<IEnumerable<AuditLog>> GetAllLogsAsync()
        {
            return await _auditLogRepository.GetAllAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetByUserNameAsync(string userName)
        {
            return await _auditLogRepository.GetByUserNameAsync(userName);
        }

        public async Task<IEnumerable<AuditLog>> GetByEntityTypeAsync(string entityType)
        {
            return await _auditLogRepository.GetByEntityTypeAsync(entityType);
        }

        public async Task<IEnumerable<AuditLog>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _auditLogRepository.GetByDateRangeAsync(startDate, endDate);
        }
    }
}
