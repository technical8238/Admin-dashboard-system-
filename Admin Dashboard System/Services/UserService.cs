using Admin_Dashboard_System.Models;
using Admin_Dashboard_System.Repositories;
using Admin_Dashboard_System.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace Admin_Dashboard_System.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserService(IUserRepository userRepository, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IEnumerable<UserViewModel>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            var userViewModels = new List<UserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userViewModels.Add(new UserViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName ?? "",
                    Email = user.Email ?? "",
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Role = roles.FirstOrDefault() ?? "User",
                    IsActive = user.IsActive,
                    CreatedDate = user.CreatedDate
                });
            }

            return userViewModels;
        }

        public async Task<UserViewModel?> GetUserByIdAsync(string id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return null;

            var roles = await _userManager.GetRolesAsync(user);
            return new UserViewModel
            {
                Id = user.Id,
                UserName = user.UserName ?? "",
                Email = user.Email ?? "",
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = roles.FirstOrDefault() ?? "User",
                IsActive = user.IsActive,
                CreatedDate = user.CreatedDate
            };
        }

        public async Task CreateUserAsync(CreateUserViewModel model)
        {
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            // Ensure role exists
            if (!await _roleManager.RoleExistsAsync(model.Role))
            {
                await _roleManager.CreateAsync(new IdentityRole(model.Role));
            }

            await _userManager.AddToRoleAsync(user, model.Role);
        }

        public async Task UpdateUserAsync(EditUserViewModel model)
        {
            var user = await _userRepository.GetByIdAsync(model.Id);
            if (user == null) throw new Exception("User not found");

            user.Email = model.Email;
            user.UserName = model.Email;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.IsActive = model.IsActive;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            // Update role
            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            if (!await _roleManager.RoleExistsAsync(model.Role))
            {
                await _roleManager.CreateAsync(new IdentityRole(model.Role));
            }

            await _userManager.AddToRoleAsync(user, model.Role);
        }

        public async Task DeleteUserAsync(string id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) throw new Exception("User not found");

            await _userRepository.DeleteAsync(user);
        }

        public async Task<int> GetTotalUsersCountAsync()
        {
            return await _userRepository.CountAsync();
        }
    }
}