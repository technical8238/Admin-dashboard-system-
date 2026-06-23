using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Admin_Dashboard_System.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Product name is required")]
        [MaxLength(100, ErrorMessage = "Product name cannot exceed 100 characters")]
        [MinLength(2, ErrorMessage = "Product name must be at least 2 characters")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, 1000000, ErrorMessage = "Price must be between 0.01 and 1,000,000")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Stock quantity is required")]
        [Range(0, 100000, ErrorMessage = "Stock quantity must be between 0 and 100,000")]
        public int StockQuantity { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}