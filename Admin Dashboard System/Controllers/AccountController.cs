using System.ComponentModel.DataAnnotations;
using Admin_Dashboard_System.Models;
using Admin_Dashboard_System.Services;
using Admin_Dashboard_System.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Admin_Dashboard_System.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IAuditLogService _auditLogService;
        private readonly IEmailService _emailService;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager, IAuditLogService auditLogService, IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _auditLogService = auditLogService;
            _emailService = emailService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                    try
                    {
                        await _auditLogService.LogActionAsync(user?.Email ?? model.Email, "Login", "Account", null, "User logged in", ipAddress);
                    }
                    catch (Exception ex)
                    {
                        // Log error but don't block login
                        Console.WriteLine($"Failed to log audit: {ex.Message}");
                    }
                    
                    return RedirectToAction("Index", "Dashboard");
                }
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // Ensure Admin role exists
                    if (!await _roleManager.RoleExistsAsync("Admin"))
                        await _roleManager.CreateAsync(new IdentityRole("Admin"));
                    if (!await _roleManager.RoleExistsAsync("Manager"))
                        await _roleManager.CreateAsync(new IdentityRole("Manager"));
                    if (!await _roleManager.RoleExistsAsync("User"))
                        await _roleManager.CreateAsync(new IdentityRole("User"));

                    await _userManager.AddToRoleAsync(user, "User");

                    // Send welcome email
                    try
                    {
                        await _emailService.SendWelcomeEmailAsync(user.Email, $"{user.FirstName} {user.LastName}");
                    }
                    catch (Exception ex)
                    {
                        // Log error but don't block registration
                        Console.WriteLine($"Failed to send welcome email: {ex.Message}");
                    }

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Dashboard");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            var user = await _userManager.GetUserAsync(User);
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            await _auditLogService.LogActionAsync(user?.Email ?? "Unknown", "Logout", "Account", null, "User logged out", ipAddress);
            
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }

    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;
    }
}