using VirtualTour.BL.Repositories;
using VirtualTour.Model;
using Microsoft.Extensions.Logging;


namespace VirtualTour.BL.Services
{
    public interface IUserService
    {
      
        Task CreateUserAsync(ReqUserCreate user);
        Task UpdateUserAsync(UpdateUserDTO user);
        Task DeleteUserAsync(string userId);
        Task<List<RepUserFetch>> GetAllUsersAsync();
        Task<RepUserFetch> GetUserAsync(string userId);
        Task<List<RepUserFetch>> GetAllUsersPagination(int pageSize, int pageNumber);
        Task AddRoleAsync(string userId, string roleId);
        Task UpdateAvatar(string userId, string avatarUrl);

    }
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;

        }
        public async Task CreateUserAsync(ReqUserCreate user)
        {
            await _userRepository.CreateUser(user);
        }

        public async Task UpdateUserAsync(UpdateUserDTO user)
        {
            await _userRepository.UpdateUser(user);
        }

        public async Task DeleteUserAsync(string userId)
        {
            await _userRepository.DeleteUser(userId);
        }
        public async Task<List<RepUserFetch>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllUser();
        }
        public async Task<RepUserFetch> GetUserAsync(string userId)
        {
            return await _userRepository.GetUserAsync(userId);
        }
        public async Task AddRoleAsync(string userId, string roleId)
        {
            await _userRepository.AddRoleAsync(userId, roleId);
        }
        public async Task<List<RepUserFetch>> GetAllUsersPagination(int pageSize, int pageNumber)
        {
            var allUsers = await _userRepository.GetAllUser();
            return allUsers.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        }
        public async Task UpdateAvatar(string userId, string avatarUrl)
        {
            await _userRepository.UpdateAvatar(userId, avatarUrl);

        }
    }
}
