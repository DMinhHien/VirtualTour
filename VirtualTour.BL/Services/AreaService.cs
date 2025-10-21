using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualTour.BL.Repositories;
using VirtualTour.Model;

namespace VirtualTour.BL.Services
{
    public interface IAreaService
    {
        Task<List<AreaModel>> GetAllAreasAsync();
        Task<AreaModel> GetAreaByIdAsync(int id);
        Task CreateAreaAsync(AreaModel area);
        Task UpdateAreaAsync(AreaModel area);
        Task DeleteAreaAsync(int id);
    }
    public class AreaService: IAreaService
    {
        private readonly IAreaRepository _areaRepository;
        public AreaService(IAreaRepository areaRepository)
        {
            _areaRepository = areaRepository;
        }
        public async Task<List<AreaModel>> GetAllAreasAsync()
        {
            return await _areaRepository.GetAllAreasAsync();
        }
        public async Task<AreaModel> GetAreaByIdAsync(int id)
        {
            return await _areaRepository.GetAreaByIdAsync(id);
        }
        public async Task CreateAreaAsync(AreaModel area)
        {
            await _areaRepository.CreateAreaAsync(area);
        }
        public async Task UpdateAreaAsync(AreaModel area)
        {
            await _areaRepository.UpdateAreaAsync(area);
        }
        public async Task DeleteAreaAsync(int id)
        {
            await _areaRepository.DeleteAreaAsync(id);
        }
    }
}
