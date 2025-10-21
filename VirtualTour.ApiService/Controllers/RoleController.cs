using VirtualTour.BL.Services;
using VirtualTour.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace VirtualTour.ApiService.Controllers
{
    [ApiController]
    [Route("api/role")]
 
    public class RoleController : ControllerBase
    {
        IRoleService _roleService;
        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }
        [Authorize]
        [HttpGet("getListUse")]
        public async Task<IActionResult> GetAll()
        {
            var roles = await _roleService.GetAllRolesAsync();
            return Ok(new BaseResponseModel { Success = true, Data = roles });
        }
        [HttpGet("getElementById/{roleId}")]
        public async Task<IActionResult> GetById(int roleId)
        {
            var role = await _roleService.GetRoleByIdAsync(roleId);
            if (role == null)
            {
                return BadRequest(new BaseResponseModel { Success = false, ErrorMessage = "Role not found" });
            }
            return Ok(new BaseResponseModel { Success = true, Data = role });
        }
        [Authorize(Policy = "Permission:Roles.Create")]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] RoleModel role)
        {
            if (role == null || string.IsNullOrEmpty(role.RoleName))
            {
                return BadRequest(new BaseResponseModel { Success = false, ErrorMessage = "Invalid role data" });
            }
            await _roleService.CreateRoleAsync(role);
            return Ok(new BaseResponseModel { Success = true });
        }
        [Authorize(Policy = "Permission:Roles.Manage")]
        [HttpPost("assignPermission/{roleId}")]
        public async Task<IActionResult> AssignPermission(int roleId, [FromBody] List<int> permissionIds)
        {
            if (permissionIds == null || !permissionIds.Any())
            {
                return BadRequest(new BaseResponseModel { Success = false, ErrorMessage = "Invalid permission IDs" });
            }
            await _roleService.AssignPermissionToRoleAsync(roleId, permissionIds);
            return Ok(new BaseResponseModel { Success = true });
        }
        [Authorize(Policy = "Permission:Roles.Edit")]
        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] RoleModel role)
        {
            if (role == null || string.IsNullOrEmpty(role.RoleName))
            {
                return BadRequest(new BaseResponseModel { Success = false, ErrorMessage = "Invalid role data" });
            }
            await _roleService.UpdateRoleAsync(role);
            return Ok(new BaseResponseModel { Success = true });
        }
        [Authorize(Policy = "Permission:Roles.Delete")]
        [HttpDelete("delete/{roleId}")]
        public async Task<IActionResult> DeleteRole(int roleId)
        {
            if (roleId <= 0)
            {
                return BadRequest(new BaseResponseModel { Success = false, ErrorMessage = "Invalid role ID" });
            }
            await _roleService.DeleteRoleAsync(roleId);
            return Ok(new BaseResponseModel { Success = true });
        }
        [Authorize(Policy = "Permission:Roles.Manage")]
        [HttpPost("deletePermission/{roleId}")]
        public async Task<IActionResult> DeletePermission(int roleId, [FromBody] List<int> permissionIds)
        {
            if (permissionIds == null || !permissionIds.Any())
            {
                return BadRequest(new BaseResponseModel { Success = false, ErrorMessage = "Invalid permission IDs" });
            }
            await _roleService.DeletePermissionFromRoleAsync(roleId, permissionIds);
            return Ok(new BaseResponseModel { Success = true });
        }
    }
}
