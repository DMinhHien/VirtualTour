using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualTour.BL.Repositories;
using VirtualTour.Model;

namespace VirtualTour.BL.Services
{
    public interface IDeptService
    {
        Task<List<DeptModel>> GetAllDeptsAsync();
        Task<DeptModel> GetDeptByIdAsync(int id);
        Task CreateDeptAsync(DeptModel dept);
        Task UpdateDeptAsync(DeptModel dept);
        Task DeleteDeptAsync(int id);
    }
    public class DeptService: IDeptService
    {
        private readonly IDeptRepository _deptRepository;
        public DeptService(IDeptRepository deptRepository)
        {
            _deptRepository = deptRepository;
        }
        public async Task<List<DeptModel>> GetAllDeptsAsync()
        {
            return await _deptRepository.GetAllDeptsAsync();
        }
        public async Task<DeptModel> GetDeptByIdAsync(int id)
        {
            return await _deptRepository.GetDeptByIdAsync(id);
        }
        public async Task CreateDeptAsync(DeptModel dept)
        {
            await _deptRepository.CreateDeptAsync(dept);
        }
        public async Task UpdateDeptAsync(DeptModel dept)
        {
            await _deptRepository.UpdateDeptAsync(dept);
        }
        public async Task DeleteDeptAsync(int id)
        {
            await _deptRepository.DeleteDeptAsync(id);
        }
    }
}
