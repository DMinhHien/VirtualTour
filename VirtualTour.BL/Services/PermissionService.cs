using VirtualTour.BL.Repositories;
using VirtualTour.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualTour.BL.Services
{
    public interface IPermissionService
    {
        Task<IEnumerable<PermissionModel>> GetAllPermissionsAsync();
        Task CreatePermissionAsync(PermissionModel permission);
        Task UpdatePermissionAsync(PermissionModel permission);
        Task DeletePermissionAsync(int permissionId);
        Task<IEnumerable<PermissionModel>> GetPermissionByRoleIdAsync(int roleId);
        Task UpdateActiveStatusAsync (int permissionId, bool isActive);
        Task<bool> CheckPermission(int roleId, string permission);
    }
    public class PermissionService: IPermissionService
    {
        private readonly IPermissionRepository _permissionRepository;
        public PermissionService(IPermissionRepository permissionRepository)
        {
            _permissionRepository = permissionRepository;
        }
        public async Task<IEnumerable<PermissionModel>> GetAllPermissionsAsync()
        {
            return await _permissionRepository.GetAllPermissionsAsync();
        }
        public async Task CreatePermissionAsync(PermissionModel permission)
        {
            await _permissionRepository.CreatePermissionAsync(permission);
        }
        public async Task UpdatePermissionAsync(PermissionModel permission)
        {
            await _permissionRepository.UpdatePermissionAsync(permission);
        }
        public async Task DeletePermissionAsync(int permissionId)
        {
            await _permissionRepository.DeletePermissionAsync(permissionId);
        }
        public async Task<IEnumerable<PermissionModel>> GetPermissionByRoleIdAsync(int roleId)
        {
            return await _permissionRepository.GetPermissionByRoleIdAsync(roleId);
        }
        public async Task UpdateActiveStatusAsync(int permissionId, bool isActive)
        {
            await _permissionRepository.UpdateActiveStatusAsync(permissionId, isActive);
        }
        public async Task<bool> CheckPermission(int roleId, string permission)
        {
            return await _permissionRepository.CheckPermission(roleId, permission);
        }

    }
}
