using Admin_Dashboard_System.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Admin_Dashboard_System.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany()
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany()
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ShoppingCart>()
                .HasOne(sc => sc.User)
                .WithMany()
                .HasForeignKey(sc => sc.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ShoppingCart>()
                .HasOne(sc => sc.Product)
                .WithMany()
                .HasForeignKey(sc => sc.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ShoppingCart>()
                .Property(sc => sc.CreatedDate)
                .HasDefaultValueSql("GETDATE()");
        }
    }
}