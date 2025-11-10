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
    public interface IFloorRepository
    {
        Task<List<FloorModel>> GetAllFloorsAsync();
        Task<FloorModel> GetFloorByIdAsync(int id);
        Task CreateFloorAsync(FloorModel floor);
        Task UpdateFloorAsync(FloorModel floor);
        Task DeleteFloorAsync(int id);
    }
    public class FloorRepository : IFloorRepository
    {
        private readonly IDbContext _dbContext;
        public FloorRepository(IDbContext dbContext)
        {
            _dbContext = dbContext;

        }
        public async Task<List<FloorModel>> GetAllFloorsAsync()
        {
            var storedProcedure = "sp_Floors_GetAll";
            using (var connection = _dbContext.CreateConnection())
            {
                List<FloorModel> floor_list = (await connection.QueryAsync<FloorModel>(storedProcedure, commandType: CommandType.StoredProcedure)).ToList();
                return floor_list;
            }
        }
        public async Task<FloorModel> GetFloorByIdAsync(int id)
        {
            var storedProcedure = "sp_Floors_GetById";
            var parameters = new DynamicParameters();
            parameters.Add("@Id", id);
            using (var connection = _dbContext.CreateConnection())
            {
                var floor = await connection.QueryFirstOrDefaultAsync<FloorModel>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
                return floor;
            }
        }
        public async Task CreateFloorAsync(FloorModel floor)
        {
            var storedProcedure = "sp_Floors_Create";
            var parameters = new DynamicParameters();
            parameters.Add("@FloorNum", floor.FloorNum);
            parameters.Add("@SectId", floor.SectId);
            using (var connection = _dbContext.CreateConnection())
            {
               await connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
 
            }
        }
        public async Task UpdateFloorAsync(FloorModel floor)
        {
            var storedProcedure = "sp_Floors_Update";
            var parameters = new DynamicParameters();
            parameters.Add("@Id", floor.Id);
            parameters.Add("@FloorNum", floor.FloorNum);
            using (var connection = _dbContext.CreateConnection())
            {
                await connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
            }
        }
        public async Task DeleteFloorAsync(int id)
        {
            var storedProcedure = "sp_Floors_Delete";
            var parameters = new DynamicParameters();
            parameters.Add("@Id", id);
            using (var connection = _dbContext.CreateConnection())
            {
                await connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
            }
        }
    }
}
