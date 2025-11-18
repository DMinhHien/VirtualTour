using Dapper;
using VirtualTour.BL.Services;
using VirtualTour.DataAccess;
using VirtualTour.Model;
using Microsoft.Data.SqlClient;
using System.Data;
namespace VirtualTour.BL.Repositories
{
    public interface IUserRepository
    {
        Task<(UserModel? user, string? error)> AuthenticateUser(string email, string password);
        Task<(UserModel? user, string? error)> AuthenticateUserWithUserName(string username, string password);
        Task SaveTokenAsync(string userId, string token, DateTime expiration);
        Task<bool> IsTokenValidAsync(string token);
        Task CreateUser(ReqUserCreate user);
        Task UpdateUser(UpdateUserDTO user);
        Task DeleteUser(string userId);
        Task<List<RepUserFetch>> GetAllUser();
        Task DeleteTokenAsync(string userId, string token);
        Task<RepUserFetch> GetUserAsync(string userId);
        Task AddRoleAsync(string userId, string roleId);
        Task ResetPassword(string userId);
        Task UpdateAvatar(string userId, string avatarUrl);
        Task<int> GetMaxId();
    }
    public class UserRepository : IUserRepository
    {
        private readonly IDbContext _dbContext;
        private readonly IPasswordService _passwordService;
        public UserRepository(IDbContext dbContext, IPasswordService passwordService, ITenantService tenantService)
        {
            _dbContext = dbContext;
            _passwordService = passwordService;
        }
        public async Task<(UserModel? user, string? error)> AuthenticateUser(string email, string password)
        {
            var storedProcedure = "sp_user_login";
            var parameters = new DynamicParameters();
            parameters.Add("@Email", email);
            try
            {
                using (var connection = _dbContext.CreateConnection())
                {
                    var user = await connection.QueryFirstOrDefaultAsync<UserModel>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
                    if (user != null)
                    {
                        bool isValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
                        if (isValid)
                        {
                            return (user, null);
                        }
                        else
                        {
                            return (null, "Invalid password.");
                        }
                    }
                    return (null, "User not found.");
                }
            }
            catch (SqlException ex)
            {
                return (null, $"{ex.Message}");
            }
        }
        public async Task<(UserModel? user, string? error)> AuthenticateUserWithUserName(string username, string password)
        {
            var storedProcedure = "sp_user_login_with_username";
            var parameters = new DynamicParameters();
            parameters.Add("@UserName", username);
            try
            {
                using (var connection = _dbContext.CreateConnection())
                {
                    var user = await connection.QueryFirstOrDefaultAsync<UserModel>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
                    if (user != null)
                    {
                        bool isValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
                        if (isValid)
                        {
                            return (user, null);
                        }
                        else
                        {
                            return (null, "Invalid password.");
                        }
                    }
                    return (null, "User not found.");
                }
            }
            catch (SqlException ex)
            {
                return (null, $"{ex.Message}");
            }
        }

        public async Task SaveTokenAsync(string userId, string token, DateTime expiration)
        {
            var storedProcedure = "sp_user_save_token";
            var parameters = new DynamicParameters();
            parameters.Add("@UserId", userId);
            parameters.Add("@Token", token);
            parameters.Add("@ExpiredDate", expiration);
            using (var connection = _dbContext.CreateConnection())
            {
                await connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
            }
        }
        public async Task DeleteTokenAsync(string userId, string token)
        {
            var storedProcedure = "sp_user_delete_token_single";
            var parameters = new DynamicParameters();
            parameters.Add("@UserId", userId);
            parameters.Add("@Token", token);
            using (var connection = _dbContext.CreateConnection())
            {
                await connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
            }
        }
        public async Task<bool> IsTokenValidAsync(string token)
        {
            var storedProcedure = "sp_user_check_token";
            var parameters = new DynamicParameters();
            parameters.Add("@Token", token);

            using (var connection = _dbContext.CreateConnection())
            {
                var result = await connection.ExecuteScalarAsync<bool>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
                return result;
            }
        }
        public async Task CreateUser(ReqUserCreate user)
        {
            var tenantId=Guid.NewGuid().ToString();
            var storedProcedure = "sp_user_create";
            var parameters = new DynamicParameters();
            parameters.Add("@Id", user.Id);
            parameters.Add("@UserName", user.UserName);
            parameters.Add("@PasswordHash", BCrypt.Net.BCrypt.HashPassword(user.PasswordHash));
            parameters.Add("@Email", user.Email);
            parameters.Add("@FullName", user.FullName);
            parameters.Add("@Gender", user.Gender);
            parameters.Add("@PhoneNumber", user.PhoneNumber);
            parameters.Add("@RoleId", user.RoleId);
            parameters.Add("@TenantId", tenantId);
            using (var connection = _dbContext.CreateConnection())
            {
                await connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
            }
        }
        public async Task UpdateUser(UpdateUserDTO user)
        {
            var storedProcedure = "sp_user_update";
            var parameters = new DynamicParameters();
            parameters.Add("@Id", user.Id);
            parameters.Add("@Email", user.Email);
            parameters.Add("@FullName", user.FullName);
            parameters.Add("@Gender", user.Gender);
            parameters.Add("@PhoneNumber", user.PhoneNumber);
            using (var connection = _dbContext.CreateConnection())
            {
                await connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
            }
        }
        public async Task DeleteUser(string userId)
        {
            var storedProcedure = "sp_user_delete";
            var parameters = new DynamicParameters();
            parameters.Add("@Id", userId);
            using (var connection = _dbContext.CreateConnection())
            {
                await connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
            }
        }
        public async Task<List<RepUserFetch>> GetAllUser()
        {
            var storedProcedure = "sp_user_get_all";
            using (var connection = _dbContext.CreateConnection())
            {
                var users = await connection.QueryAsync<RepUserFetch>(storedProcedure, commandType: CommandType.StoredProcedure);
                foreach (var user in users)
                {
                    if (!string.IsNullOrEmpty(user.HashKey))
                    {
                        user.HashKey = _passwordService.DecryptString(user.HashKey);
                    }
                }
                return users.ToList();
            }
        }
        public async Task<RepUserFetch> GetUserAsync(string userId)
        {
            var storedProcedure = "sp_user_get_by_id";
            var parameters = new DynamicParameters();
            parameters.Add("@Id", userId);
            using (var connection = _dbContext.CreateConnection())
            {
                var user = await connection.QueryFirstOrDefaultAsync<RepUserFetch>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
                return user;
            }
        }
        public async Task AddRoleAsync(string userId, string roleId)
        {
            var storedProcedure = "sp_user_add_role";
            var parameters = new DynamicParameters();
            parameters.Add("@UserId", userId);
            parameters.Add("@RoleId", roleId);
            using (var connection = _dbContext.CreateConnection())
            {
                await connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
            }
        }
        public async Task ResetPassword(string userId)
        {
            var storedProcedure = "sp_user_reset_password";
            var parameters = new DynamicParameters();
            parameters.Add("@Id", userId);
            parameters.Add("@NewPasswordHash", BCrypt.Net.BCrypt.HashPassword("123456"));
            using (var connection = _dbContext.CreateConnection())
            {
                await connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
            }
        }
        public async Task UpdateAvatar(string userId, string avatarUrl)
        {
            var storedProcedure = "sp_user_update_avatar";
            var parameters = new DynamicParameters();
            parameters.Add("@Id", userId);
            parameters.Add("@AvatarUrl", avatarUrl);
            using (var connection = _dbContext.CreateConnection())
            {
                await connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
            }
        }
        public async Task<int> GetMaxId()
        {
            var sql = "SELECT MAX(id) AS max_id FROM UserManage;";
            try
            {
                using (var connection = _dbContext.CreateConnection())
                {
                    var maxId = await connection.QuerySingleAsync<int?>(sql, commandType: CommandType.Text);
                    return maxId ?? 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching max node ID: {ex.Message}");
            }
        }
    }

}
