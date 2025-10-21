using Dapper;
using VirtualTour.DataAccess;
using VirtualTour.Model;
using Microsoft.Data.SqlClient;
using System.Data;

namespace VirtualTour.BL.Repositories
{
    public interface IRoleRepository
    {
        public Task CreateRoleAsync(RoleModel role);
        public Task<IEnumerable<RoleModel>> GetAllRolesAsync();
        public Task<RoleModel> GetRoleByIdAsync(int roleId);
        public Task AddPermissionsToRoleAsync(int roleId, List<int> permissionIds);
        public Task GetRolePermissionsAsync(int roleId);
        public Task UpdateRoleAsync(RoleModel role);
        public Task DeleteRoleAsync(int roleId);
        public Task DeletePermissionsFromRoleAsync(int roleId, List<int> permissionIds);
    }
    public class RoleRepository : IRoleRepository
    {
        private readonly IDbContext _dbContext;
        public RoleRepository(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task CreateRoleAsync(RoleModel role)
        {
            var storedProcedure = "sp_role_create";
            var parameters = new DynamicParameters();
            parameters.Add("@Id", role.Id);
            parameters.Add("@RoleName", role.RoleName);
            try
            {
                using (var connection = _dbContext.CreateConnection())
                {
                    await connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
                }
            }
            catch (SqlException ex)
            {
                throw new Exception($"Error creating role: {ex.Message}");
            }
        }
        public async Task<IEnumerable<RoleModel>> GetAllRolesAsync()
        {
            var storedProcedure = "sp_role_get_all";
            try
            {
                using (var connection = _dbContext.CreateConnection())
                {
                    return await connection.QueryAsync<RoleModel>(storedProcedure, commandType: CommandType.StoredProcedure);
                }
            }
            catch (SqlException ex)
            {
                throw new Exception($"Error fetching roles: {ex.Message}");
            }
        }
        public async Task<RoleModel> GetRoleByIdAsync(int roleId)
        {
            var storedProcedure = "sp_role_get_by_id";
            var parameters = new DynamicParameters();
            parameters.Add("@RoleId", roleId);
            try
            {
                using (var connection = _dbContext.CreateConnection())
                {
                    return await connection.QueryFirstOrDefaultAsync<RoleModel>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
                }
            }
            catch (SqlException ex)
            {
                throw new Exception($"Error fetching role by ID: {ex.Message}");
            }
        }
        public async Task AddPermissionsToRoleAsync(int roleId, List<int> permissionIds)
        {
            foreach (var permissionId in permissionIds)
            {
                var storedProcedure = "sp_role_add_permission";
                var parameters = new DynamicParameters();
                parameters.Add("@RoleId", roleId);
                parameters.Add("@PermissionId", permissionId);
                try
                {
                    using (var connection = _dbContext.CreateConnection())
                    {
                        await connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception($"Error adding permission to role: {ex.Message}");
                }
            }

        }
        public async Task GetRolePermissionsAsync(int roleId)
        {
            var storedProcedure = "sp_role_get_permissions";
            var parameters = new DynamicParameters();
            parameters.Add("@RoleId", roleId);
            try
            {
                using (var connection = _dbContext.CreateConnection())
                {
                    await connection.QueryAsync<PermissionModel>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
                }
            }
            catch (SqlException ex)
            {
                throw new Exception($"Error fetching role permissions: {ex.Message}");
            }
        }
        public async Task UpdateRoleAsync(RoleModel role)
        {
            var storedProcedure = "sp_role_update";
            var parameters = new DynamicParameters();
            parameters.Add("@Id", role.Id);
            parameters.Add("@RoleName", role.RoleName);
            try
            {
                using (var connection = _dbContext.CreateConnection())
                {
                    await connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
                }
            }
            catch (SqlException ex)
            {
                throw new Exception($"Error updating role: {ex.Message}");
            }
        }
        public async Task DeleteRoleAsync(int roleId)
        {
            var storedProcedure = "sp_role_delete";
            var parameters = new DynamicParameters();
            parameters.Add("@Id", roleId);
            try
            {
                using (var connection = _dbContext.CreateConnection())
                {
                    await connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
                }
            }
            catch (SqlException ex)
            {
                throw new Exception($"Error deleting role: {ex.Message}");
            }
        }
        public async Task DeletePermissionsFromRoleAsync(int roleId, List<int> permissionIds)
        {
            foreach (var permissionId in permissionIds)
            {
                var storedProcedure = "sp_role_delete_permission";
                var parameters = new DynamicParameters();
                parameters.Add("@RoleId", roleId);
                parameters.Add("@PermissionId", permissionId);
                try
                {
                    using (var connection = _dbContext.CreateConnection())
                    {
                        await connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception($"Error deleting permission from role: {ex.Message}");
                }
            }
        }
    }
}
