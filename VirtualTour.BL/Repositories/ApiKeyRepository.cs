using Dapper;
using VirtualTour.DataAccess;
using VirtualTour.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace VirtualTour.BL.Repositories
{
    public interface IApiKeyRepository
    {
        Task<ApiKeyModel?> GetApiKeyByHashAsync(string hashKey);
        Task<ApiKeyModel?> GetApiKeyByUserIdAsync(int userId);
        Task CreateApiKeyAsync(ApiKeyModel apiKey);
        Task UpdateApiKeyAsync(ApiKeyModel apiKey);
    }
    public class ApiKeyRepository : IApiKeyRepository
    {
        private readonly IDbContext _dbContext;
        public ApiKeyRepository(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ApiKeyModel?> GetApiKeyByHashAsync(string hashKey)
        {
            try
            {
                using (var connection = _dbContext.CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@HashKey", hashKey, DbType.String);
                    return await connection.QueryFirstOrDefaultAsync<ApiKeyModel>("sp_apiKey_getByHash", parameters, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching API key: {ex.Message}");
            }
        }

        public async Task<ApiKeyModel?> GetApiKeyByUserIdAsync(int userId)
        {
            try
            {
                using (var connection = _dbContext.CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@UserId", userId, DbType.Int32);
                    return await connection.QueryFirstOrDefaultAsync<ApiKeyModel>("sp_apiKey_getByUserId", parameters, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching API key for UserId {userId}: {ex.Message}");
            }
        }

        public async Task CreateApiKeyAsync(ApiKeyModel apiKey)
        {
            try
            {
                using (var connection = _dbContext.CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@UserId", apiKey.UserId, DbType.Int32);
                    parameters.Add("@HashKey", apiKey.HashKey, DbType.String);
                    parameters.Add("@ExpiredAt", apiKey.ExpiredAt, DbType.DateTime);
                    await connection.ExecuteAsync("sp_apiKey_create", parameters, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating API key: {ex.Message}");
            }
        }

        public async Task UpdateApiKeyAsync(ApiKeyModel apiKey)
        {
            try
            {
                using (var connection = _dbContext.CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@UserId", apiKey.UserId, DbType.Int32);
                    parameters.Add("@HashKey", apiKey.HashKey, DbType.String);
                    parameters.Add("@ExpiredAt", apiKey.ExpiredAt, DbType.DateTime);
                    await connection.ExecuteAsync("sp_apiKey_update", parameters, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating API key: {ex.Message}");
            }
        }
    }
}
