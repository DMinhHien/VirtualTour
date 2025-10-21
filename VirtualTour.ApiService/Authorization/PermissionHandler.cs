using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace VirtualTour.ApiService.Authorization
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IPermissionChecker _permissionChecker;
        public PermissionHandler(IPermissionChecker permissionChecker)
        {
            _permissionChecker = permissionChecker;
        }
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            var user = context.User;
            if (user?.Identity?.IsAuthenticated != true) 
                return;
            if (await _permissionChecker.HasPermissionAsync(requirement.Permission))
            {
                context.Succeed(requirement);
            }
        }
    }
}
