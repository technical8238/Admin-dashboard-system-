using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Admin_Dashboard_System.Models
{
    public class AuditLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Action { get; set; } = string.Empty; // Create, Update, Delete, Login, Logout

        [Required]
        [MaxLength(100)]
        public string EntityType { get; set; } = string.Empty; // User, Product, Category, Order

        public int? EntityId { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(100)]
        public string? IpAddress { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
