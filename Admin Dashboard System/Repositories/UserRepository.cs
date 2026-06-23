using Admin_Dashboard_System.Data;
using Admin_Dashboard_System.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Admin_Dashboard_System.Repositories
{
    public interface IUserRepository
    {
        Task<ApplicationUser?> GetByIdAsync(string id);
        Task<IEnumerable<ApplicationUser>> GetAllAsync();
        Task<ApplicationUser> AddAsync(ApplicationUser user);
        Task UpdateAsync(ApplicationUser user);
        Task DeleteAsync(ApplicationUser user);
        Task<int> CountAsync();
        Task<bool> ExistsAsync(string email);
    }

    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserRepository(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<ApplicationUser?> GetByIdAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<ApplicationUser> AddAsync(ApplicationUser user)
        {
            await _userManager.CreateAsync(user);
            return user;
        }

        public async Task UpdateAsync(ApplicationUser user)
        {
            await _userManager.UpdateAsync(user);
        }

        public async Task DeleteAsync(ApplicationUser user)
        {
            await _userManager.DeleteAsync(user);
        }

        public async Task<int> CountAsync()
        {
            return await _context.Users.CountAsync();
        }

        public async Task<bool> ExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }
    }
}