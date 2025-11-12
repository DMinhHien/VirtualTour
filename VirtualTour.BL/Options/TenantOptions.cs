namespace VirtualTour.BL.Options
{
    public class TenantOptions
    {
        // List of valid tenant identifiers
        public List<string> AllowedTenants { get; set; } = new();
        // Fallback when no tenant supplied or invalid
        public string DefaultTenantId { get; set; } = "default";
        // Header name (can override in config if needed)
        public string HeaderName { get; set; } = "X-Tenant-ID";
        // Query string key
        public string QueryStringKey { get; set; } = "tenantId";
        // Claim type to check
        public string ClaimType { get; set; } = "tenant_id";
    }
}