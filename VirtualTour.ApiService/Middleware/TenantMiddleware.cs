
using Microsoft.Extensions.Options;
using VirtualTour.BL.Options;
using VirtualTour.BL.Services;

namespace VirtualTour.ApiService.Middleware
{
    public class TenantMiddleware
    {
        private readonly RequestDelegate _next;

        public TenantMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context,
                                      ITenantService tenantService,
                                      IOptions<TenantOptions> tenantOptions)
        {
            var opts = tenantOptions.Value;

            // 1. Header
            string? tenantId = context.Request.Headers[opts.HeaderName].FirstOrDefault();

            // 2. Claim (available after authentication; if middleware placed before auth this may be null)
            if (string.IsNullOrWhiteSpace(tenantId))
            {
                tenantId = context.User?.FindFirst(opts.ClaimType)?.Value;
            }

            // 3. Query string
            if (string.IsNullOrWhiteSpace(tenantId))
            {
                tenantId = context.Request.Query[opts.QueryStringKey].FirstOrDefault();
            }

            // 4. Fallback default
            if (string.IsNullOrWhiteSpace(tenantId))
            {
                tenantId = opts.DefaultTenantId;
            }

            // Validate
            //if (!tenantService.IsValidTenant(tenantId))
            //{
            //    // Option: short-circuit with 400/403. For now fallback to default.
            //    tenantId = opts.DefaultTenantId;
            //}

            tenantService.SetTenantId(tenantId);

            // OPTIONAL: expose to downstream components (e.g., for logging)
            context.Response.OnStarting(() =>
            {
                context.Response.Headers["X-Resolved-Tenant"] = tenantId!;
                return Task.CompletedTask;
            });

            await _next(context);
        }
    }
}
