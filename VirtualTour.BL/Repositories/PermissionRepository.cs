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
    public interface IPermissionRepository
    {
        Task<IEnumerable<PermissionModel>> GetAllPermissionsAsync();
        Task CreatePermissionAsync(PermissionModel permission);
        Task UpdatePermissionAsync(PermissionModel permission);
        Task DeletePermissionAsync(int permissionId);
        Task<IEnumerable<PermissionModel>> GetPermissionByRoleIdAsync(int roleId);
        Task UpdateActiveStatusAsync(int permissionId,bool IsActive);
        Task<bool> CheckPermission (int roleId,string permission);
    }
    public class PermissionRepository : IPermissionRepository
    {
        private readonly IDbContext _dbContext;
        public PermissionRepository(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<IEnumerable<PermissionModel>> GetAllPermissionsAsync()
        {
            var storedProcedure = "sp_permission_get_all";
            try
            {
                using (var connection = _dbContext.CreateConnection())
                {
                    return await connection.QueryAsync<PermissionModel>(storedProcedure, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching permissions: {ex.Message}");
            }
        }
        public async Task CreatePermissionAsync(PermissionModel permission)
        {
            var storedProcedure = "sp_permission_create";
            var parameters = new DynamicParameters();
            parameters.Add("@PermissionName", permission.PermissionName);
            try
            {
                using (var connection = _dbContext.CreateConnection())
                {
                    await connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating permission: {ex.Message}");
            }
        }
        public async Task UpdatePermissionAsync(PermissionModel permission)
        {
            var storedProcedure = "sp_permission_update";
            var parameters = new DynamicParameters();
            parameters.Add("@Id", permission.Id);
            parameters.Add("@PermissionName", permission.PermissionName);
            try
            {
                using (var connection = _dbContext.CreateConnection())
                {
                    await connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating permission: {ex.Message}");
            }
        }
        public async Task DeletePermissionAsync(int permissionId)
        {
            var storedProcedure = "sp_permission_delete";
            var parameters = new DynamicParameters();
            parameters.Add("@Id", permissionId);
            try
            {
                using (var connection = _dbContext.CreateConnection())
                {
                    await connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting permission: {ex.Message}");
            }
        }
        public async Task<IEnumerable<PermissionModel>> GetPermissionByRoleIdAsync(int roleId)
        {
            var storedProcedure = "sp_permission_get_by_role_id";
            var parameters = new DynamicParameters();
            parameters.Add("@RoleId", roleId);
            try
            {
                using (var connection = _dbContext.CreateConnection())
                {
                    return await connection.QueryAsync<PermissionModel>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching permission by role ID: {ex.Message}");
            }
        }
        public async Task UpdateActiveStatusAsync(int permissionId, bool isActive)
        {
            var storedProcedure = "sp_permission_update_active_status";
            var parameters = new DynamicParameters();
            parameters.Add("@Id", permissionId);
            parameters.Add("@IsActive", isActive);
            try
            {
                using (var connection = _dbContext.CreateConnection())
                {
                    await connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating active status: {ex.Message}");
            }
        }
        public async Task<bool> CheckPermission(int roleId, string permission)
        {
            var storedProcedure = "sp_permission_check";
            var parameters = new DynamicParameters();
            parameters.Add("@RoleId", roleId);
            parameters.Add("@Permission", permission);
            try
            {
                using (var connection = _dbContext.CreateConnection())
                {
                    return await connection.ExecuteScalarAsync<bool>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error checking permission: {ex.Message}");
            }
        }
    }
}
