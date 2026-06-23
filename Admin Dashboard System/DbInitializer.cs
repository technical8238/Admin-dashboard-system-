using Admin_Dashboard_System.Data;
using Admin_Dashboard_System.Models;
using Microsoft.AspNetCore.Identity;

namespace Admin_Dashboard_System
{
    public static class DbInitializer
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            context.Database.EnsureCreated();

            // Create roles
            string[] roleNames = { "Admin", "Manager", "User" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Create admin user
            if (await userManager.FindByEmailAsync("admin@example.com") == null)
            {
                var adminUser = new ApplicationUser
                {
                    UserName = "admin@example.com",
                    Email = "admin@example.com",
                    FirstName = "Admin",
                    LastName = "User",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // Seed categories
            if (!context.Categories.Any())
            {
                var categories = new List<Category>
                {
                    new Category { Name = "Electronics", Description = "Electronic devices and gadgets", IsActive = true },
                    new Category { Name = "Clothing", Description = "Fashion and apparel", IsActive = true },
                    new Category { Name = "Books", Description = "Books and publications", IsActive = true },
                    new Category { Name = "Home & Garden", Description = "Home improvement and garden supplies", IsActive = true }
                };

                context.Categories.AddRange(categories);
                context.SaveChanges();
            }

            // Seed products
            if (!context.Products.Any())
            {
                var electronics = context.Categories.First(c => c.Name == "Electronics");
                var clothing = context.Categories.First(c => c.Name == "Clothing");
                var books = context.Categories.First(c => c.Name == "Books");

                var products = new List<Product>
                {
                    new Product { Name = "Laptop", Description = "High-performance laptop", Price = 999.99m, StockQuantity = 50, CategoryId = electronics.Id, IsActive = true },
                    new Product { Name = "Smartphone", Description = "Latest smartphone model", Price = 699.99m, StockQuantity = 100, CategoryId = electronics.Id, IsActive = true },
                    new Product { Name = "T-Shirt", Description = "Cotton t-shirt", Price = 19.99m, StockQuantity = 200, CategoryId = clothing.Id, IsActive = true },
                    new Product { Name = "Programming Book", Description = "Learn ASP.NET Core", Price = 49.99m, StockQuantity = 75, CategoryId = books.Id, IsActive = true }
                };

                context.Products.AddRange(products);
                context.SaveChanges();
            }
        }
    }
}