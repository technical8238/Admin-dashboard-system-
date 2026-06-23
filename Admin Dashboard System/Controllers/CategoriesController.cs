using Admin_Dashboard_System.Services;
using Admin_Dashboard_System.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Admin_Dashboard_System.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class CategoriesController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index(string searchTerm, string sortOrder, string statusFilter, int page = 1, int pageSize = 10)
        {
            var categories = await _categoryService.GetAllCategoriesAsync();

            // Search
            if (!string.IsNullOrEmpty(searchTerm))
            {
                categories = categories.Where(c => 
                    c.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    c.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
                ViewData["SearchTerm"] = searchTerm;
            }

            // Filters
            if (!string.IsNullOrEmpty(statusFilter) && statusFilter != "All")
            {
                var isActive = statusFilter == "Active";
                categories = categories.Where(c => c.IsActive == isActive);
                ViewData["StatusFilter"] = statusFilter;
            }

            // Sorting
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParam"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["ProductCountSortParam"] = sortOrder == "count_asc" ? "count_desc" : "count_asc";
            ViewData["DateSortParam"] = sortOrder == "date_asc" ? "date_desc" : "date_asc";

            categories = sortOrder switch
            {
                "name_desc" => categories.OrderByDescending(c => c.Name),
                "count_asc" => categories.OrderBy(c => c.ProductCount),
                "count_desc" => categories.OrderByDescending(c => c.ProductCount),
                "date_asc" => categories.OrderBy(c => c.CreatedDate),
                "date_desc" => categories.OrderByDescending(c => c.CreatedDate),
                _ => categories.OrderBy(c => c.Name)
            };

            var paginatedCategories = PaginatedList<CategoryViewModel>.Create(categories, page, pageSize);
            return View(paginatedCategories);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _categoryService.CreateCategoryAsync(model);
                    TempData["Success"] = "Category created successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            var model = new EditCategoryViewModel
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                IsActive = category.IsActive
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _categoryService.UpdateCategoryAsync(model);
                    TempData["Success"] = "Category updated successfully.";
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
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _categoryService.DeleteCategoryAsync(id);
                TempData["Success"] = "Category deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }
    }
}