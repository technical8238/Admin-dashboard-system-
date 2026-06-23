using Admin_Dashboard_System.Services;
using Admin_Dashboard_System.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Admin_Dashboard_System.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly IUserService _userService;
        private readonly IExportService _exportService;

        public UsersController(IUserService userService, IExportService exportService)
        {
            _userService = userService;
            _exportService = exportService;
        }

        public async Task<IActionResult> Index(string searchTerm, string sortOrder, string roleFilter, string statusFilter, int page = 1, int pageSize = 10)
        {
            var users = await _userService.GetAllUsersAsync();

            // Search
            if (!string.IsNullOrEmpty(searchTerm))
            {
                users = users.Where(u => 
                    u.FirstName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    u.LastName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    u.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
                ViewData["SearchTerm"] = searchTerm;
            }

            // Filters
            if (!string.IsNullOrEmpty(roleFilter) && roleFilter != "All")
            {
                users = users.Where(u => u.Role == roleFilter);
                ViewData["RoleFilter"] = roleFilter;
            }

            if (!string.IsNullOrEmpty(statusFilter) && statusFilter != "All")
            {
                var isActive = statusFilter == "Active";
                users = users.Where(u => u.IsActive == isActive);
                ViewData["StatusFilter"] = statusFilter;
            }

            // Sorting
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParam"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["EmailSortParam"] = sortOrder == "email_asc" ? "email_desc" : "email_asc";
            ViewData["RoleSortParam"] = sortOrder == "role_asc" ? "role_desc" : "role_asc";
            ViewData["DateSortParam"] = sortOrder == "date_asc" ? "date_desc" : "date_asc";

            users = sortOrder switch
            {
                "name_desc" => users.OrderByDescending(u => u.FirstName).ThenByDescending(u => u.LastName),
                "email_asc" => users.OrderBy(u => u.Email),
                "email_desc" => users.OrderByDescending(u => u.Email),
                "role_asc" => users.OrderBy(u => u.Role),
                "role_desc" => users.OrderByDescending(u => u.Role),
                "date_asc" => users.OrderBy(u => u.CreatedDate),
                "date_desc" => users.OrderByDescending(u => u.CreatedDate),
                _ => users.OrderBy(u => u.FirstName).ThenBy(u => u.LastName)
            };

            var paginatedUsers = PaginatedList<UserViewModel>.Create(users, page, pageSize);
            return View(paginatedUsers);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _userService.CreateUserAsync(model);
                    TempData["Success"] = "User created successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var model = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role,
                IsActive = user.IsActive
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _userService.UpdateUserAsync(model);
                    TempData["Success"] = "User updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _userService.DeleteUserAsync(id);
                TempData["Success"] = "User deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ExportToExcel()
        {
            var users = await _userService.GetAllUsersAsync();
            var excelData = _exportService.ExportUsersToExcel(users);
            return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "users.xlsx");
        }

        public async Task<IActionResult> ExportToPdf()
        {
            var users = await _userService.GetAllUsersAsync();
            var pdfData = _exportService.ExportUsersToPdf(users);
            return File(pdfData, "application/pdf", "users.pdf");
        }
    }
}