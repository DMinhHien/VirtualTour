using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace VirtualTour.ApiService.Authorization
{
    public class PermissionPolicyProvider : IAuthorizationPolicyProvider
    {
        const string POLICY_PREFIX = "Permission";
        private readonly DefaultAuthorizationPolicyProvider _fallbackPolicyProvider;

        public PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
        {
            _fallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
        }
        // Default policy must be non-nullable
        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
            => _fallbackPolicyProvider.GetDefaultPolicyAsync();

        // Fallback policy can be nullable
        public Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
            => _fallbackPolicyProvider.GetFallbackPolicyAsync();

        public async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            // if the policy follows our convention
            if (policyName.StartsWith(POLICY_PREFIX, System.StringComparison.OrdinalIgnoreCase))
            {
                var permissionName = policyName.Substring(POLICY_PREFIX.Length + 1); // e.g. "Permission:Users.Edit"
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .AddRequirements(new PermissionRequirement(permissionName))
                    .Build();
                return policy;
            }
            // otherwise fall back to default policies (e.g. roles)
            return await _fallbackPolicyProvider.GetPolicyAsync(policyName);
        }
    }
}
