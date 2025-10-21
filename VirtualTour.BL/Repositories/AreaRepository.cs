using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualTour.DataAccess;
using VirtualTour.Model;

namespace VirtualTour.BL.Repositories
{
    public interface IAreaRepository
    {
        Task<List<AreaModel>> GetAllAreasAsync();
        Task<AreaModel> GetAreaByIdAsync(int id);
        Task CreateAreaAsync(AreaModel area);
        Task UpdateAreaAsync(AreaModel area);
        Task DeleteAreaAsync(int id);

    }
    public class AreaRepository: IAreaRepository
    {
        private readonly IDbContext _dbContext;
        public AreaRepository(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<AreaModel>> GetAllAreasAsync()
        {
            var storedProcedure = "[dbo].[sp_Areas_GetAll]";
            using (var connection = _dbContext.CreateConnection())
            {
                List<AreaModel> area_list = (await connection.QueryAsync<AreaModel>(storedProcedure, commandType: CommandType.StoredProcedure)).ToList();
                return area_list;
            }
        }
        public async Task<AreaModel> GetAreaByIdAsync(int id)
        {
            var storedProcedure = "sp_Areas_GetById";
            var parameters = new DynamicParameters();
            parameters.Add("@Id", id);
            using (var connection = _dbContext.CreateConnection())
            {
                var area = await connection.QueryFirstOrDefaultAsync<AreaModel>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
                return area;
            }
        }
        public async Task CreateAreaAsync(AreaModel area)
        {
            var storedProcedure = "sp_Areas_Create";
            var parameters = new DynamicParameters();
            parameters.Add("@AreaName", area.AreaName);
            using (var connection = _dbContext.CreateConnection())
            {
                await connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
            }
        }
        public async Task UpdateAreaAsync(AreaModel area)
        {
            var storedProcedure = "sp_Areas_Update";
            var parameters = new DynamicParameters();
            parameters.Add("@Id", area.Id);
            parameters.Add("@AreaName", area.AreaName);
            using (var connection = _dbContext.CreateConnection())
            {
                await connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
            }
        }
        public async Task DeleteAreaAsync(int id)
        {
            var storedProcedure = "sp_Areas_Delete";
            var parameters = new DynamicParameters();
            parameters.Add("@Id", id);
            using (var connection = _dbContext.CreateConnection())
            {
                await connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
            }
        }
    }
}
