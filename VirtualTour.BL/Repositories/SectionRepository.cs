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
    public interface ISectionRepository
    {
        Task<List<SectionModel>> GetAllSectionsAsync();
        Task<SectionModel> GetSectionByIdAsync(int id);
        Task CreateSectionAsync(SectionModel section);
        Task UpdateSectionAsync(SectionModel section);
        Task DeleteSectionAsync(int id);
    }
    public class SectionRepository: ISectionRepository
    {
        private readonly IDbContext _dbContext;
        public SectionRepository(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<SectionModel>> GetAllSectionsAsync()
        {
            var storedProcedure = "[dbo].[sp_Sections_GetAll]";
            using (var connection = _dbContext.CreateConnection())
            {
                List<SectionModel> section_list = (await connection.QueryAsync<SectionModel>(storedProcedure, commandType: CommandType.StoredProcedure)).ToList();
                return section_list;
            }
        }
        public async Task<SectionModel> GetSectionByIdAsync(int id)
        {
            var storedProcedure = "sp_Sections_GetById";
            var parameters = new DynamicParameters();
            parameters.Add("@Id", id);
            using (var connection = _dbContext.CreateConnection())
            {
                var section = await connection.QueryFirstOrDefaultAsync<SectionModel>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
                return section;
            }
        }
        public async Task CreateSectionAsync(SectionModel section)
        {
            var storedProcedure = "sp_Sections_Create";
            var parameters = new DynamicParameters();
            parameters.Add("@SectName", section.SectName);
            using (var connection = _dbContext.CreateConnection())
            {
              await connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);

            }
        }
        public async Task UpdateSectionAsync(SectionModel section)
        {
            var storedProcedure = "sp_Sections_Update";
            var parameters = new DynamicParameters();
            parameters.Add("@Id", section.Id);
            parameters.Add("@SectName", section.SectName);
            using (var connection = _dbContext.CreateConnection())
            {
                await connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
            }
        }
        public async Task DeleteSectionAsync(int id)
        {
            var storedProcedure = "sp_Sections_Delete";
            var parameters = new DynamicParameters();
            parameters.Add("@Id", id);
            using (var connection = _dbContext.CreateConnection())
            {
                await connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
            }
        }

    }
}
