using VirtualTour.BL.Services;
using VirtualTour.Model;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace VirtualTour.ApiService.Data
{
    public static class SeedData
    {
   
        private static readonly string[] DefaultPermissions = new[]
        {
            "Users.Manage",
            "Users.Edit",
            "Users.Delete",
            "Users.Create",
            "Roles.Manage",
            "Permission.Manage",
            "Admin.View",
            "Roles.Create",
            "Roles.Edit",
            "Roles.Delete",
            "QRCode.Manage",
            "QRCode.Create",
            "QRCode.Edit",
            "QRCode.Delete",
            "VirtualTour.Manage",

        };

        public static async Task InitializePermissionsAsync(IServiceProvider services)
        {
            var permService = services.GetRequiredService<IPermissionService>();
            var existing = (await permService.GetAllPermissionsAsync()).Select(p => p.PermissionName).ToHashSet(StringComparer.OrdinalIgnoreCase);
            foreach (var name in DefaultPermissions)
            {
                if (!existing.Contains(name))
                {
                    var permission = new PermissionModel
                    {
                        PermissionName = name,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        IsActive = true
                    };
                    await permService.CreatePermissionAsync(permission);
                }
            }
        }
 
    }
}
