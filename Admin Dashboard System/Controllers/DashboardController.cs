using Admin_Dashboard_System.Services;
using Admin_Dashboard_System.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Admin_Dashboard_System.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public async Task<IActionResult> Index()
        {
            var data = await _dashboardService.GetDashboardDataAsync();
            return View(data);
        }

        public async Task<IActionResult> Reports()
        {
            var data = await _dashboardService.GetDashboardDataAsync();
            return View(data);
        }
    }
}