using VirtualTour.BL.Repositories;
using VirtualTour.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualTour.BL.Services
{
    public interface IAuthService
    {
        Task<(UserModel? user, string? error)> AuthenticateUserAsync(string email, string password);
        Task<(UserModel? user, string? error)> AuthenticateUserWithUserNameAsync(string email, string password);
        Task SaveTokenAsync(string userId, string token, DateTime expiration);
        Task DeleteTokenAsync(string userId, string token);
        Task<bool> IsTokenValidAsync(string token);
        Task ResetPassword(string userId);
    }
    public class AuthService : IAuthService
    {
        public readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;
        public AuthService(IUserRepository userRepository, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }
        public async Task<(UserModel? user, string? error)> AuthenticateUserAsync(string email, string password)
        {

            var (user, error) = await _userRepository.AuthenticateUser(email, password);
            if (user == null)
            {
                _logger.LogWarning("Authentication failed: {error}", error);
            }
            return (user, error);
        }
        public async Task SaveTokenAsync(string userId, string token, DateTime expiration)
        {
            try
            {
                await _userRepository.SaveTokenAsync(userId, token, expiration);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving token for user {userId}", userId);
                throw;
            }
        }
        public async Task DeleteTokenAsync(string userId, string token)
        {
            try
            {
                await _userRepository.DeleteTokenAsync(userId,token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting token for user {userId}", userId);
                throw;
            }
        }
        public async Task<bool> IsTokenValidAsync(string token)
        {
            try
            {
                return await _userRepository.IsTokenValidAsync(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating token {token}", token);
                throw;
            }

        }
        public async Task ResetPassword(string userId)
        {
            await _userRepository.ResetPassword(userId);
        }
        public async Task<(UserModel? user, string? error)> AuthenticateUserWithUserNameAsync(string username, string password)
        {
            var (user, error) = await _userRepository.AuthenticateUserWithUserName(username, password);
            if (user == null)
            {
                _logger.LogWarning("Authentication failed for username {username}: {error}", username, error);
            }
            return (user, error);
        }
    }
}