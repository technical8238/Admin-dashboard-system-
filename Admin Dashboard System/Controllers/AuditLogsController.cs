using Admin_Dashboard_System.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Admin_Dashboard_System.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AuditLogsController : Controller
    {
        private readonly IAuditLogService _auditLogService;

        public AuditLogsController(IAuditLogService auditLogService)
        {
            _auditLogService = auditLogService;
        }

        public async Task<IActionResult> Index(string? userNameFilter, string? entityTypeFilter, DateTime? startDate, DateTime? endDate)
        {
            var logs = await _auditLogService.GetAllLogsAsync();

            // Filters
            if (!string.IsNullOrEmpty(userNameFilter))
            {
                logs = logs.Where(l => l.UserName.Contains(userNameFilter, StringComparison.OrdinalIgnoreCase));
                ViewData["UserNameFilter"] = userNameFilter;
            }

            if (!string.IsNullOrEmpty(entityTypeFilter) && entityTypeFilter != "All")
            {
                logs = logs.Where(l => l.EntityType == entityTypeFilter);
                ViewData["EntityTypeFilter"] = entityTypeFilter;
            }

            if (startDate.HasValue)
            {
                logs = logs.Where(l => l.CreatedDate >= startDate.Value);
                ViewData["StartDate"] = startDate.Value.ToString("yyyy-MM-dd");
            }

            if (endDate.HasValue)
            {
                logs = logs.Where(l => l.CreatedDate <= endDate.Value);
                ViewData["EndDate"] = endDate.Value.ToString("yyyy-MM-dd");
            }

            logs = logs.OrderByDescending(l => l.CreatedDate).Take(500); // Limit to last 500 logs
            return View(logs);
        }
    }
}
