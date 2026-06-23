# Admin Dashboard System

A complete production-ready Admin Dashboard System built with ASP.NET Core MVC (.NET 8) following clean architecture and best practices.

## 🏗️ Project Structure

```
Admin Dashboard System/
├── Controllers/
│   ├── AccountController.cs          # Authentication
│   ├── DashboardController.cs        # Main dashboard
│   ├── UsersController.cs           # User management
│   ├── CategoriesController.cs      # Category management
│   ├── ProductsController.cs        # Product management
│   ├── OrdersController.cs          # Order management
│   └── ProductsApiController.cs     # REST API for products
├── Data/
│   └── ApplicationDbContext.cs      # EF Core context
├── Models/
│   ├── ApplicationUser.cs           # Identity user
│   ├── Category.cs                  # Category entity
│   ├── Product.cs                   # Product entity
│   └── Order.cs                     # Order entities
├── ViewModels/
│   ├── UserViewModels.cs            # User-related VMs
│   ├── CategoryViewModels.cs        # Category VMs
│   ├── ProductViewModels.cs         # Product VMs
│   ├── OrderViewModels.cs           # Order VMs
│   └── DashboardViewModels.cs       # Dashboard VM
├── Services/
│   ├── IUserService.cs & UserService.cs
│   ├── ICategoryService.cs & CategoryService.cs
│   ├── IProductService.cs & ProductService.cs
│   ├── IOrderService.cs & OrderService.cs
│   └── IDashboardService.cs & DashboardService.cs
├── Repositories/
│   ├── IRepository.cs & Repository.cs
│   ├── IUserRepository.cs & UserRepository.cs
│   ├── ICategoryRepository.cs & CategoryRepository.cs
│   ├── IProductRepository.cs & ProductRepository.cs
│   └── IOrderRepository.cs & OrderRepository.cs
├── Views/
│   ├── Shared/
│   │   ├── _Layout.cshtml
│   │   └── _AdminLayout.cshtml
│   ├── Account/ (Login, Register, AccessDenied)
│   ├── Dashboard/ (Index)
│   ├── Users/ (Index, Create, Edit)
│   ├── Categories/ (Index, Create, Edit)
│   ├── Products/ (Index, Create, Edit)
│   └── Orders/ (Index, Details)
├── wwwroot/
│   ├── css/
│   │   ├── site.css
│   │   └── admin.css
│   └── js/
│       ├── site.js
│       └── admin.js
├── Program.cs                        # Application startup
├── DbInitializer.cs                  # Database seeding
└── appsettings.json                  # Configuration
```

## 🚀 Step-by-Step Setup Guide

### Prerequisites
- .NET 8.0 SDK
- SQL Server (LocalDB or full instance)
- Visual Studio 2022 or VS Code

### 1. Database Setup
```bash
# Create migration (if EF tools work)
dotnet ef migrations add InitialCreate

# Update database
dotnet ef database update
```

### 2. Run the Application
```bash
# Restore packages
dotnet restore

# Build
dotnet build

# Run
dotnet run
```

### 3. Access the Application
- **URL**: https://localhost:5001 (or configured port)
- **Default Admin Account**:
  - Email: admin@example.com
  - Password: Admin123!

## 🎯 Key Features Implemented

### ✅ Authentication & Authorization
- ASP.NET Core Identity with custom ApplicationUser
- Role-based access (Admin, Manager, User)
- Login/Register/Logout functionality
- Protected admin routes

### ✅ Dashboard
- Statistics cards (Users, Products, Orders, Revenue)
- Interactive charts using Chart.js
- Recent activity feed
- Responsive design

### ✅ CRUD Modules
- **Users**: Create, Read, Update, Delete with role management
- **Categories**: Full CRUD with product count
- **Products**: CRUD with category association, search
- **Orders**: View details, update status

### ✅ UI/UX
- Bootstrap 5 responsive design
- Professional admin sidebar layout
- Dark mode toggle
- Toast notifications
- Form validation (client + server)

### ✅ Architecture
- Clean Architecture with separation of concerns
- Repository pattern
- Service layer
- Dependency injection
- Async/await throughout

### ✅ Database
- SQL Server with EF Core Code First
- Proper relationships and constraints
- Sample data seeding

### ✅ API
- RESTful API for Products
- JSON responses
- Swagger-ready structure

### ✅ Security
- Password hashing
- Anti-forgery tokens
- Authentication middleware
- Authorization attributes

### ✅ Additional Features
- Search functionality
- Sorting and pagination ready
- Export capabilities (Excel/PDF libraries included)
- Error handling and logging
- Professional code quality

## 🔧 Configuration

**appsettings.json**:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=AdminDashboardDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

## 📊 Sample Data Seeded
- Admin user
- 4 categories (Electronics, Clothing, Books, Home & Garden)
- Sample products
- Roles (Admin, Manager, User)

## 🎨 UI Screenshots (Conceptual)
- **Login Page**: Clean form with validation
- **Dashboard**: Cards with metrics, charts, recent activity
- **User Management**: Data table with actions
- **Product CRUD**: Forms with category dropdown
- **Responsive Sidebar**: Collapsible navigation

## 🏆 Production-Ready Aspects
- **Scalable Architecture**: Clean separation allows easy extension
- **Security**: Proper authentication, authorization, validation
- **Performance**: Async operations, efficient queries
- **Maintainability**: Well-organized code, SOLID principles
- **User Experience**: Professional UI, responsive design
- **Error Handling**: Comprehensive validation and error management
- **Database Design**: Normalized schema with relationships

This project demonstrates enterprise-level ASP.NET Core development skills and is suitable for portfolio presentation or job interviews. The code follows industry best practices and can be easily extended with additional features like email notifications, advanced reporting, or multi-tenancy.

## 📦 NuGet Packages Used

- **Microsoft.AspNetCore.Identity.EntityFrameworkCore** (8.0.0) - User authentication and authorization
- **Microsoft.EntityFrameworkCore.SqlServer** (8.0.0) - SQL Server database provider
- **Microsoft.EntityFrameworkCore.Tools** (8.0.0) - EF Core CLI tools
- **Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation** (8.0.0) - Runtime compilation
- **Microsoft.VisualStudio.Web.CodeGeneration.Design** (8.0.0) - Scaffolding tools
- **ClosedXML** (0.102.1) - Excel export functionality
- **itext7** (8.0.2) - PDF export functionality

## 🔑 Default Credentials

- **Email**: admin@example.com
- **Password**: Admin123!

## � API Endpoints

### Products API
- `GET /api/Products` - Get all products
- `GET /api/Products/{id}` - Get product by ID
- `POST /api/Products` - Create new product (Admin/Manager only)
- `PUT /api/Products/{id}` - Update product (Admin/Manager only)
- `DELETE /api/Products/{id}` - Delete product (Admin only)

## 🎯 Role Permissions

- **Admin**: Full access to all features including user management
- **Manager**: Access to categories, products, and orders (no user management)
- **User**: Read-only access to dashboard

## 🚀 Running the Application

1. Open the solution in Visual Studio 2022 or VS Code
2. Restore NuGet packages: `dotnet restore`
3. Build the solution: `dotnet build`
4. Run the application: `dotnet run`
5. Navigate to `https://localhost:5001` (or the port shown in the console)

## 📂 Database Migration Steps

If you need to recreate the database:

```bash
# Remove existing database (if any)
# Delete the .mdf file from: %localappdata%\Microsoft\Microsoft SQL Server Local DB\Instances\MSSQLLocalDB

# Create a new migration
dotnet ef migrations add InitialCreate

# Update the database
dotnet ef database update
```

## 🎨 Customization

- **Theme**: Toggle dark mode using the sidebar button
- **Pagination**: Default page size is 10, adjustable in controllers
- **Export**: Excel and PDF exports available on list pages
- **Search**: Search functionality available on Products page

## 🐛 Troubleshooting

**Database Connection Issues**:
- Ensure SQL Server LocalDB is installed
- Check connection string in `appsettings.json`
- Verify the database exists at the specified location

**Build Errors**:
- Run `dotnet clean` then `dotnet build`
- Ensure all NuGet packages are restored

## 📞 Support

For issues or questions, please refer to the code comments or ASP.NET Core documentation.

---

**Built with ❤️ using ASP.NET Core 8.0**