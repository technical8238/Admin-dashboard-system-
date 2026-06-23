using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Admin_Dashboard_System.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Category name is required")]
        [MaxLength(50, ErrorMessage = "Category name cannot exceed 50 characters")]
        [MinLength(2, ErrorMessage = "Category name must be at least 2 characters")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(200, ErrorMessage = "Description cannot exceed 200 characters")]
        public string? Description { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        // Navigation
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}