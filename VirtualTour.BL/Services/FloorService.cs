using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualTour.BL.Repositories;
using VirtualTour.Model;

namespace VirtualTour.BL.Services
{
    public interface IFloorService
    {
        Task<List<FloorModel>> GetAllFloorsAsync();
        Task<FloorModel> GetFloorByIdAsync(int id);
        Task CreateFloorAsync(FloorModel floor);
        Task UpdateFloorAsync(FloorModel floor);
        Task DeleteFloorAsync(int id);
    }
    public class FloorService: IFloorService
    {
        private readonly IFloorRepository _floorRepository;
        public FloorService(IFloorRepository floorRepository)
        {
            _floorRepository = floorRepository;
        }
        public async Task<List<FloorModel>> GetAllFloorsAsync()
        {
            return await _floorRepository.GetAllFloorsAsync();
        }
        public async Task<FloorModel> GetFloorByIdAsync(int id)
        {
            return await _floorRepository.GetFloorByIdAsync(id);
        }
        public async Task CreateFloorAsync(FloorModel floor)
        {
            await _floorRepository.CreateFloorAsync(floor);
        }
        public async Task UpdateFloorAsync(FloorModel floor)
        {
            await _floorRepository.UpdateFloorAsync(floor);
        }
        public async Task DeleteFloorAsync(int id)
        {
            await _floorRepository.DeleteFloorAsync(id);
        }
    }
}
