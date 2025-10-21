using VirtualTour.BL.Services;
using VirtualTour.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace fKVPortalAspireWeb.ApiService.Controllers
{
    [ApiController]
    [Route("api/permission")]
    public class PermissionController : ControllerBase
    {
        private readonly IPermissionService permissionService;
        public PermissionController(IPermissionService permissionService)
        {
            this.permissionService = permissionService;
        }
        [Authorize(Policy = "Permission:Permission.Manage")]
        [HttpGet("getListUse")]
        public async Task<IActionResult> GetAll()
        {
            var permissions = await permissionService.GetAllPermissionsAsync();
            return Ok(new BaseResponseModel { Success = true, Data = permissions });
        }
        [Authorize(Policy = "Permission:Permission.Manage")]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] PermissionModel permission)
        {
            if (permission == null || string.IsNullOrEmpty(permission.PermissionName))
            {
                return BadRequest(new BaseResponseModel { Success = false, ErrorMessage = "Invalid permission data" });
            }
            await permissionService.CreatePermissionAsync(permission);
            return Ok(new BaseResponseModel { Success = true });
        }
        [Authorize]
        [HttpGet("getListByRoleId/{roleId}")]
        public async Task<IActionResult> GetByRoleId(int roleId)
        {
            if (roleId <= 0)
            {
                return BadRequest(new BaseResponseModel { Success = false, ErrorMessage = "Invalid role ID" });
            }
            var permissions = await permissionService.GetPermissionByRoleIdAsync(roleId);
            return Ok(new BaseResponseModel { Success = true, Data = permissions });
        }
        [Authorize(Policy = "Permission:Permission.Manage")]
        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] PermissionModel permission)
        {
            if (permission == null || string.IsNullOrEmpty(permission.PermissionName))
            {
                return BadRequest(new BaseResponseModel { Success = false, ErrorMessage = "Invalid permission data" });
            }
            await permissionService.UpdatePermissionAsync(permission);
            return Ok(new BaseResponseModel { Success = true });
        }
        [Authorize(Policy = "Permission:Permission.Manage")]
        [HttpDelete("delete/{permissionId}")]
        public async Task<IActionResult> Delete(int permissionId)
        {
            if (permissionId <= 0)
            {
                return BadRequest(new BaseResponseModel { Success = false, ErrorMessage = "Invalid permission ID" });
            }
            await permissionService.DeletePermissionAsync(permissionId);
            return Ok(new BaseResponseModel { Success = true });
        }
        [Authorize(Policy = "Permission:Permission.Manage")]
        [HttpPut("updateActiveStatus/{roleId}")]
        public async Task<IActionResult> UpdateActiveStatus(int roleId, [FromBody] bool isActive)
        {
            if (roleId <= 0)
            {
                return BadRequest(new BaseResponseModel { Success = false, ErrorMessage = "Invalid role ID" });
            }
            await permissionService.UpdateActiveStatusAsync(roleId, isActive);
            return Ok(new BaseResponseModel { Success = true });
        }
    }
}
