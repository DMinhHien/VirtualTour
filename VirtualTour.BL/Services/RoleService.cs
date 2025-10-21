using VirtualTour.BL.Repositories;
using VirtualTour.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualTour.BL.Services
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleModel>> GetAllRolesAsync();
        Task<RoleModel> GetRoleByIdAsync(int roleId);
        Task CreateRoleAsync(RoleModel role);
        Task AssignPermissionToRoleAsync(int roleId, List<int> permissionIds);
        Task UpdateRoleAsync(RoleModel role);
        Task DeleteRoleAsync(int roleId);
        Task DeletePermissionFromRoleAsync(int roleId, List<int> permissionIds);
    }
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }
        public async Task<IEnumerable<RoleModel>> GetAllRolesAsync()
        {
            return await _roleRepository.GetAllRolesAsync();
        }
        public async Task<RoleModel> GetRoleByIdAsync(int roleId)
        {
            return await _roleRepository.GetRoleByIdAsync(roleId);
        }
        public async Task CreateRoleAsync(RoleModel role)
        {
            await _roleRepository.CreateRoleAsync(role);
        }
        public async Task AssignPermissionToRoleAsync(int roleId, List<int> permissionIds)
        {
            await _roleRepository.AddPermissionsToRoleAsync(roleId, permissionIds);
        }
        public async Task UpdateRoleAsync(RoleModel role)
        {
            await _roleRepository.UpdateRoleAsync(role);
        }
        public async Task DeleteRoleAsync(int roleId)
        {
            await _roleRepository.DeleteRoleAsync(roleId);
        }
        public async Task DeletePermissionFromRoleAsync(int roleId, List<int> permissionIds)
        {
            await _roleRepository.DeletePermissionsFromRoleAsync(roleId, permissionIds);
        }
    }
}
