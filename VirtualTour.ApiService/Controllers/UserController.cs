using VirtualTour.BL.Services;
using VirtualTour.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
namespace VirtualTour.ApiService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IApiKeyService _apiKeyService;
        public UserController(IUserService userService, IApiKeyService apiKeyService)
        {
            _userService = userService;
            _apiKeyService = apiKeyService;
        }
        [Authorize(Policy = "Permission:Users.Manage")]
        [HttpGet("getListUse")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                return Ok(new BaseResponseModel { Success = true, Data = users });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseResponseModel { Success = false, ErrorMessage = ex.Message });
            }
        }
        [Authorize(Policy = "Permission:Users.Create")]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] ReqUserCreate user)
        {
            try
            {
                if (user == null)
                {
                    return BadRequest(new BaseResponseModel { Success = false, ErrorMessage = "Invalid user data." });
                }
                await _userService.CreateUserAsync(user);
                //await _apiKeyService.GenerateApiKeyAsync(user.Id);
                return Ok(new BaseResponseModel { Success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseResponseModel { Success = false, ErrorMessage = ex.Message });
            }
        }
        //[Authorize(Policy = "Permission:Users.Edit")]
        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] UpdateUserDTO user)
        {
            try
            {
                if (user == null)
                {
                    return StatusCode(500, new BaseResponseModel { Success = false, ErrorMessage = "Invalid user data." });
                }
                await _userService.UpdateUserAsync(user);
                await _userService.AddRoleAsync(user.Id.ToString(), user.RoleId);
                return Ok(new BaseResponseModel { Success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseResponseModel { Success = false, ErrorMessage = ex.Message });
            }
        }
        [Authorize(Policy = "Permission:Users.Delete")]
        [HttpDelete("delete/{userId}")]
        public async Task<IActionResult> Delete(string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest(new BaseResponseModel { Success = false, ErrorMessage = "Invalid user ID." });
                }
                await _userService.DeleteUserAsync(userId);
                return Ok(new BaseResponseModel { Success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseResponseModel { Success = false, ErrorMessage = ex.Message });
            }
        }
        [HttpGet("getElementById/{userId}")]
        public async Task<IActionResult> GetElementById(string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest(new BaseResponseModel { Success = false, ErrorMessage = "Invalid user ID." });
                }
                var user = await _userService.GetUserAsync(userId);
                if (user == null)
                {
                    return NotFound(new BaseResponseModel { Success = false, ErrorMessage = "User not found." });
                }
                return Ok(new BaseResponseModel { Success = true, Data = user });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseResponseModel { Success = false, ErrorMessage = ex.Message });
            }
        }
        [Authorize(Policy = "Permission:Roles.Manage")]
        [HttpPost("addRole/{userId}")]
        public async Task<IActionResult> AddRole(string userId, [FromBody] string roleId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(roleId))
                {
                    return BadRequest(new BaseResponseModel { Success = false, ErrorMessage = "Invalid user or role ID." });
                }
                await _userService.AddRoleAsync(userId, roleId);
                return Ok(new BaseResponseModel { Success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseResponseModel { Success = false, ErrorMessage = ex.Message });
            }
        }
        [HttpPut("updateAvatar/{userId}")]
        public async Task<IActionResult> UpdateAvatar(string userId, [FromBody] string avatarUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(avatarUrl))
                {
                    return BadRequest(new BaseResponseModel { Success = false, ErrorMessage = "Invalid user ID or avatar URL." });
                }
                await _userService.UpdateAvatar(userId, avatarUrl);
                return Ok(new BaseResponseModel { Success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseResponseModel { Success = false, ErrorMessage = ex.Message });
            }
        }
        [HttpPost("refreshApiKey/{userId}")]
        public async Task<IActionResult> RefreshApiKey(int userId)
        {
            try
            {
                if (userId <= 0)
                {
                    return BadRequest(new BaseResponseModel { Success = false, ErrorMessage = "Invalid user ID." });
                }
                ApiKeyModel apiKeyModel = new ApiKeyModel
                {
                    UserId = userId,
                    HashKey = await _apiKeyService.GenerateRandomApiKey(50),
                    ExpiredAt = DateTime.Now.AddDays(7),
                };
                await _apiKeyService.UpdateApiKeyAsync(apiKeyModel);
                return Ok(new BaseResponseModel { Success = true });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseResponseModel { Success = false, ErrorMessage = ex.Message });

            }
        }
 
    }
}
