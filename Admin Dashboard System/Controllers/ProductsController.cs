using Admin_Dashboard_System.Hubs;
using Admin_Dashboard_System.Services;
using Admin_Dashboard_System.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Admin_Dashboard_System.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;
        private readonly IExportService _exportService;
        private readonly IHubContext<DashboardHub> _hubContext;

        public ProductsController(IProductService productService, IExportService exportService, IHubContext<DashboardHub> hubContext)
        {
            _productService = productService;
            _exportService = exportService;
            _hubContext = hubContext;
        }

        public async Task<IActionResult> Index(string searchTerm, string sortOrder, int page = 1, int pageSize = 10)
        {
            IEnumerable<ProductViewModel> products;
            if (!string.IsNullOrEmpty(searchTerm))
            {
                products = await _productService.SearchProductsAsync(searchTerm);
                ViewData["SearchTerm"] = searchTerm;
            }
            else
            {
                products = await _productService.GetAllProductsAsync();
            }

            // Sorting
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParam"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["PriceSortParam"] = sortOrder == "price_asc" ? "price_desc" : "price_asc";
            ViewData["DateSortParam"] = sortOrder == "date_asc" ? "date_desc" : "date_asc";

            products = sortOrder switch
            {
                "name_desc" => products.OrderByDescending(p => p.Name),
                "price_asc" => products.OrderBy(p => p.Price),
                "price_desc" => products.OrderByDescending(p => p.Price),
                "date_asc" => products.OrderBy(p => p.CreatedDate),
                "date_desc" => products.OrderByDescending(p => p.CreatedDate),
                _ => products.OrderBy(p => p.Name)
            };

            var paginatedProducts = PaginatedList<ProductViewModel>.Create(products, page, pageSize);
            return View(paginatedProducts);
        }

        public async Task<IActionResult> Create()
        {
            var model = await _productService.GetCreateProductViewModelAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _productService.CreateProductAsync(model);
                    await _hubContext.Clients.All.SendAsync("ProductUpdated", new { productId = 0, productName = model.Name, timestamp = DateTime.UtcNow });
                    TempData["Success"] = "Product created successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            model.Categories = (await _productService.GetCreateProductViewModelAsync()).Categories;
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var model = await _productService.GetEditProductViewModelAsync(id);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _productService.UpdateProductAsync(model);
                    await _hubContext.Clients.All.SendAsync("ProductUpdated", new { productId = model.Id, productName = model.Name, timestamp = DateTime.UtcNow });
                    TempData["Success"] = "Product updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            model.Categories = (await _productService.GetEditProductViewModelAsync(model.Id)).Categories;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _productService.DeleteProductAsync(id);
            TempData["Success"] = "Product deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkDelete(List<int> ids)
        {
            if (ids != null && ids.Any())
            {
                foreach (var id in ids)
                {
                    await _productService.DeleteProductAsync(id);
                }
                TempData["Success"] = $"{ids.Count} product(s) deleted successfully.";
            }
            else
            {
                TempData["Error"] = "No products selected.";
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ExportToExcel()
        {
            var products = await _productService.GetAllProductsAsync();
            var excelData = _exportService.ExportProductsToExcel(products);
            return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "products.xlsx");
        }

        public async Task<IActionResult> ExportToPdf()
        {
            var products = await _productService.GetAllProductsAsync();
            var pdfData = _exportService.ExportProductsToPdf(products);
            return File(pdfData, "application/pdf", "products.pdf");
        }
    }
}