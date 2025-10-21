using VirtualTour.BL.Services;

namespace VirtualTour.ApiService.Authorization
{
    public interface IPermissionChecker
    {
        Task<bool> HasPermissionAsync(string permission);
    }

    public class PermissionChecker : IPermissionChecker
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPermissionService _permService;

        public PermissionChecker(
            IHttpContextAccessor httpContextAccessor,
            IPermissionService permService)
        {
            _httpContextAccessor = httpContextAccessor;
            _permService = permService;
        }

        public async Task<bool> HasPermissionAsync(string permission)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user?.Identity?.IsAuthenticated != true)
                return false;
            var roleIds = user.Claims
                .Where(c => c.Type == "roleid")
                .Select(c => int.TryParse(c.Value, out var id) ? id : 0)
                .Where(id => id > 0);

            foreach (var roleId in roleIds)
            {
                return await _permService.CheckPermission (roleId, permission);
            }
            return false;    
         }
    }
}
