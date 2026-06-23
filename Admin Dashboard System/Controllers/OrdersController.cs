using Admin_Dashboard_System.Services;
using Admin_Dashboard_System.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Admin_Dashboard_System.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IExportService _exportService;

        public OrdersController(IOrderService orderService, IExportService exportService)
        {
            _orderService = orderService;
            _exportService = exportService;
        }

        public async Task<IActionResult> Index(string searchTerm, string sortOrder, string statusFilter, DateTime? startDate, DateTime? endDate, int page = 1, int pageSize = 10)
        {
            var orders = await _orderService.GetAllOrdersAsync();

            // Search
            if (!string.IsNullOrEmpty(searchTerm))
            {
                orders = orders.Where(o => 
                    o.UserName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    o.UserEmail.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
                ViewData["SearchTerm"] = searchTerm;
            }

            // Filters
            if (!string.IsNullOrEmpty(statusFilter) && statusFilter != "All")
            {
                orders = orders.Where(o => o.Status == statusFilter);
                ViewData["StatusFilter"] = statusFilter;
            }

            if (startDate.HasValue)
            {
                orders = orders.Where(o => o.OrderDate >= startDate.Value);
                ViewData["StartDate"] = startDate.Value.ToString("yyyy-MM-dd");
            }

            if (endDate.HasValue)
            {
                orders = orders.Where(o => o.OrderDate <= endDate.Value);
                ViewData["EndDate"] = endDate.Value.ToString("yyyy-MM-dd");
            }

            // Sorting
            ViewData["CurrentSort"] = sortOrder;
            ViewData["IdSortParam"] = string.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
            ViewData["CustomerSortParam"] = sortOrder == "customer_asc" ? "customer_desc" : "customer_asc";
            ViewData["AmountSortParam"] = sortOrder == "amount_asc" ? "amount_desc" : "amount_asc";
            ViewData["StatusSortParam"] = sortOrder == "status_asc" ? "status_desc" : "status_asc";
            ViewData["DateSortParam"] = sortOrder == "date_asc" ? "date_desc" : "date_asc";

            orders = sortOrder switch
            {
                "id_desc" => orders.OrderByDescending(o => o.Id),
                "customer_asc" => orders.OrderBy(o => o.UserName),
                "customer_desc" => orders.OrderByDescending(o => o.UserName),
                "amount_asc" => orders.OrderBy(o => o.TotalAmount),
                "amount_desc" => orders.OrderByDescending(o => o.TotalAmount),
                "status_asc" => orders.OrderBy(o => o.Status),
                "status_desc" => orders.OrderByDescending(o => o.Status),
                "date_asc" => orders.OrderBy(o => o.OrderDate),
                "date_desc" => orders.OrderByDescending(o => o.OrderDate),
                _ => orders.OrderByDescending(o => o.Id)
            };

            var paginatedOrders = PaginatedList<OrderViewModel>.Create(orders, page, pageSize);
            return View(paginatedOrders);
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderService.GetOrderDetailsAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            try
            {
                await _orderService.UpdateOrderStatusAsync(id, status);
                TempData["Success"] = "Order status updated successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _orderService.DeleteOrderAsync(id);
                TempData["Success"] = "Order deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ExportToExcel()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            var excelData = _exportService.ExportOrdersToExcel(orders);
            return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "orders.xlsx");
        }

        public async Task<IActionResult> ExportToPdf()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            var pdfData = _exportService.ExportOrdersToPdf(orders);
            return File(pdfData, "application/pdf", "orders.pdf");
        }
    }
}