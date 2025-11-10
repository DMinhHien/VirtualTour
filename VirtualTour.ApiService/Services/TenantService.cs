using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using VirtualTour.ApiService.Options;

namespace VirtualTour.ApiService.Services
{
    public interface ITenantService
    {
        string? GetCurrentTenantId();
        void SetTenantId(string tenantId);
        bool HasTenant();
        bool IsValidTenant(string tenantId);
    }
    public class TenantService : ITenantService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly TenantOptions _options;
        private const string TenantItemKey = "__TenantId";

        // Simple in-memory cache of allowed tenants (optional perf boost)
        private static readonly ConcurrentDictionary<string, byte> _tenantCache = new();

        public TenantService(IHttpContextAccessor accessor, IOptions<TenantOptions> options)
        {
            _httpContextAccessor = accessor;
            _options = options.Value;
            foreach (var t in _options.AllowedTenants)
                _tenantCache.TryAdd(t, 0);
        }

        public string? GetCurrentTenantId()
        {
            var ctx = _httpContextAccessor.HttpContext;
            if (ctx?.Items.TryGetValue(TenantItemKey, out var value) == true)
                return value as string;
            return null;
        }

        public void SetTenantId(string tenantId)
        {
            var ctx = _httpContextAccessor.HttpContext;
            if (ctx == null) return;
            ctx.Items[TenantItemKey] = tenantId;
        }

        public bool HasTenant() => !string.IsNullOrWhiteSpace(GetCurrentTenantId());

        public bool IsValidTenant(string tenantId)
        {
            if (string.IsNullOrWhiteSpace(tenantId)) return false;
            if (_tenantCache.ContainsKey(tenantId)) return true;
            return _options.AllowedTenants.Contains(tenantId, StringComparer.OrdinalIgnoreCase);
        }
    }
}
