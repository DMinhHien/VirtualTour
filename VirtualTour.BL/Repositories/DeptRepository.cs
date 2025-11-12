using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualTour.BL.Services;
using VirtualTour.DataAccess;
using VirtualTour.Model;

namespace VirtualTour.BL.Repositories
{
    public interface IDeptRepository
    {
        Task<List<DeptModel>> GetAllDeptsAsync();
        Task<DeptModel> GetDeptByIdAsync(int id);
        Task CreateDeptAsync(DeptModel dept);
        Task UpdateDeptAsync(DeptModel dept);
        Task DeleteDeptAsync(int id);

    }
    public class DeptRepository : IDeptRepository
    {
        private readonly IDbContext _dbContext;
        private readonly ITenantService tenantService;
        public DeptRepository(IDbContext dbContext, ITenantService tenantService)
        {
            _dbContext = dbContext;
            this.tenantService = tenantService;
        }
        public async Task<List<DeptModel>> GetAllDeptsAsync()
        {
            var tenantId = tenantService.GetCurrentTenantId();
            var storedProcedure = "sp_Depts_GetAll";
            var parameters = new DynamicParameters();
            parameters.Add("@TenantId", tenantId);
            using (var connection = _dbContext.CreateConnection())
            {
                List<DeptModel> dept_list = (await connection.QueryAsync<DeptModel>(storedProcedure,parameters, commandType: System.Data.CommandType.StoredProcedure)).ToList();
                return dept_list;
            }
        }
        public async Task<DeptModel> GetDeptByIdAsync(int id)
        {
            var tenantId = tenantService.GetCurrentTenantId();
            var storedProcedure = "sp_Depts_GetById";
            var parameters = new Dapper.DynamicParameters();
            parameters.Add("@Id", id);
            parameters.Add("@TenantId", tenantId);
            using (var connection = _dbContext.CreateConnection())
            {
                var dept = await connection.QueryFirstOrDefaultAsync<DeptModel>(storedProcedure, parameters, commandType: System.Data.CommandType.StoredProcedure);
                return dept;
            }
        }
        public async Task CreateDeptAsync(DeptModel dept)
        {
            var tenantId = tenantService.GetCurrentTenantId();
            var storedProcedure = "sp_Depts_Create";
            var parameters = new Dapper.DynamicParameters();
            parameters.Add("@DeptName", dept.DeptName);
            parameters.Add("@AreaId", dept.AreaId);
            parameters.Add("@TenantId", tenantId);
            using (var connection = _dbContext.CreateConnection())
            {
               await connection.ExecuteAsync(storedProcedure, parameters, commandType: System.Data.CommandType.StoredProcedure);
            }
        }
        public async Task UpdateDeptAsync(DeptModel dept)
        {
            var storedProcedure = "sp_Depts_Update";
            var parameters = new Dapper.DynamicParameters();
            parameters.Add("@Id", dept.Id);
            parameters.Add("@DeptName", dept.DeptName);
            using (var connection = _dbContext.CreateConnection())
            {
                await connection.ExecuteAsync(storedProcedure, parameters, commandType: System.Data.CommandType.StoredProcedure);
            }
        }
        public async Task DeleteDeptAsync(int id)
        {
            var storedProcedure = "sp_Depts_Delete";
            var parameters = new Dapper.DynamicParameters();
            parameters.Add("@Id", id);
            using (var connection = _dbContext.CreateConnection())
            {
                await connection.ExecuteAsync(storedProcedure, parameters, commandType: System.Data.CommandType.StoredProcedure);
            }
        }
    }

}
