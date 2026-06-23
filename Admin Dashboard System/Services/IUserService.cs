using Admin_Dashboard_System.ViewModels;

namespace Admin_Dashboard_System.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserViewModel>> GetAllUsersAsync();
        Task<UserViewModel?> GetUserByIdAsync(string id);
        Task CreateUserAsync(CreateUserViewModel model);
        Task UpdateUserAsync(EditUserViewModel model);
        Task DeleteUserAsync(string id);
        Task<int> GetTotalUsersCountAsync();
    }
}