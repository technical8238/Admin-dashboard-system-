using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Admin_Dashboard_System.ViewModels
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class CreateProductViewModel
    {
        [Required(ErrorMessage = "Product name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Product name must be between 2 and 100 characters")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, 1000000, ErrorMessage = "Price must be between 0.01 and 1,000,000")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Stock quantity is required")]
        [Range(0, 100000, ErrorMessage = "Stock quantity must be between 0 and 100,000")]
        public int StockQuantity { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public int CategoryId { get; set; }

        [AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png", ".gif" }, ErrorMessage = "Only JPG, JPEG, PNG, and GIF files are allowed")]
        [MaxFileSize(5 * 1024 * 1024, ErrorMessage = "File size cannot exceed 5MB")]
        public IFormFile? ImageFile { get; set; }

        public List<CategoryViewModel> Categories { get; set; } = new();
    }

    public class EditProductViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Product name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Product name must be between 2 and 100 characters")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, 1000000, ErrorMessage = "Price must be between 0.01 and 1,000,000")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Stock quantity is required")]
        [Range(0, 100000, ErrorMessage = "Stock quantity must be between 0 and 100,000")]
        public int StockQuantity { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public int CategoryId { get; set; }

        public bool IsActive { get; set; }

        public string? ExistingImageUrl { get; set; }

        [AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png", ".gif" }, ErrorMessage = "Only JPG, JPEG, PNG, and GIF files are allowed")]
        [MaxFileSize(5 * 1024 * 1024, ErrorMessage = "File size cannot exceed 5MB")]
        public IFormFile? ImageFile { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; } = new List<SelectListItem>();
    }

    public class AllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] _extensions;

        public AllowedExtensionsAttribute(string[] extensions)
        {
            _extensions = extensions;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                var extension = Path.GetExtension(file.FileName).ToLower();
                if (!_extensions.Contains(extension))
                {
                    return new ValidationResult(ErrorMessage);
                }
            }
            return ValidationResult.Success;
        }
    }

    public class MaxFileSizeAttribute : ValidationAttribute
    {
        private readonly int _maxFileSize;

        public MaxFileSizeAttribute(int maxFileSize)
        {
            _maxFileSize = maxFileSize;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                if (file.Length > _maxFileSize)
                {
                    return new ValidationResult(ErrorMessage);
                }
            }
            return ValidationResult.Success;
        }
    }
}