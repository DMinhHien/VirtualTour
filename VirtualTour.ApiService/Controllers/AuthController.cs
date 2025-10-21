using VirtualTour.BL.Services;
using VirtualTour.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace VirtualTour.ApiService.Controllers
{
    [ApiController]
    [Route("api/auth")]

    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ITokenService _tokenService;
        private readonly IApiKeyService _apiKeyService;

        public AuthController(IAuthService authService, ITokenService tokenService, IApiKeyService apiKeyService)
        {
            _authService = authService;
            _tokenService = tokenService;
            _apiKeyService = apiKeyService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] ReqLoginDTO loginRequest)
        {
            var (user, error) = await _authService.AuthenticateUserAsync(loginRequest.Email, loginRequest.Password);

            if (user == null)
            {
                return Unauthorized(new { message = error ?? "Invalid credentials" });
            }
            var token = _tokenService.CreateToken(user, new List<string> { user.RoleName });
            var expiration = DateTime.Now.AddDays(7);
            await _authService.SaveTokenAsync(user.Id.ToString(), token, expiration);

            var loginResponse = new LoginResponse
            {
                Token = token,
                UserName = user.UserName,
                Email = user.Email,
                FullName = user.FullName,
                Gender = user.Gender,
                PhoneNumber = user.PhoneNumber,
                Role = user.RoleName,
                AvatarUrl=user.AvatarUrl,
                CompanyEmail=user.CompanyEmail,
                ManagerName=user.ManagerName,
            };

            return Ok(loginResponse); ;
        }
        [HttpPost("login_username")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWithUserName([FromBody] ReqLoginDTO loginRequest)
        {
            var (user, error) = await _authService.AuthenticateUserWithUserNameAsync(loginRequest.Email, loginRequest.Password);
            if (user == null)
            {
                return Unauthorized(new { message = error ?? "Invalid credentials" });
            }
            var token = _tokenService.CreateToken(user, new List<string> { user.RoleName });
            var expiration = DateTime.Now.AddDays(7);
            await _authService.SaveTokenAsync(user.Id.ToString(), token, expiration);
            ApiKeyModel apiKey = new ApiKeyModel();
            apiKey=await _apiKeyService.GetApiKeyByUserIdAsync(int.Parse( user.Id.ToString()));
            if (apiKey == null || apiKey.ExpiredAt < DateTime.Now ||apiKey.IsRevoked)
            {
                return Unauthorized(new { message = error ?? "Invalid credentials" });
            }
            var loginResponse = new LoginResponse
            {
                Token = token,
                UserName = user.UserName,
                Email = user.Email,
                FullName = user.FullName,
                Gender = user.Gender,
                PhoneNumber = user.PhoneNumber,
                Role = user.RoleName,
                AvatarUrl = user.AvatarUrl,
                ApiKey = apiKey.HashKey,
                CompanyEmail = user.CompanyEmail,
                ManagerName = user.ManagerName,
            };

            return Ok(loginResponse); ;
        }
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var authHeader = Request.Headers["Authorization"].FirstOrDefault();
            string token = authHeader.Substring("Bearer ".Length).Trim();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return BadRequest(new { message = "User not found" });
            }
            await _authService.DeleteTokenAsync(userId,token);
            return Ok(new { message = "Logged out successfully" });
        }
        [HttpGet("check-token")]
        public async Task<IActionResult> CheckToken()
        {
            var authHeader = Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return Ok(false);
            }

            string token = authHeader.Substring("Bearer ".Length).Trim();

            var isValid = await _authService.IsTokenValidAsync(token);
            return Ok(isValid);
        }
        [HttpPost("reset-password")]
        [Authorize(Policy = "Permission:Users.View")]
        public async Task<IActionResult> ResetPassword([FromBody] string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new BaseResponseModel { Success = false, ErrorMessage = "User Id is required" });
            }
            try
            {
                await _authService.ResetPassword(userId);
                return Ok(new BaseResponseModel { Success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseResponseModel { Success = false, ErrorMessage = ex.Message });
            }
        }
    }
}
