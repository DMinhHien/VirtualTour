using VirtualTour.Model;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json;
using System.Threading;

namespace VirtualTour.Web.Authorization
{
    public interface IPermissionChecker
    {
        Task InitializeAsync();
        bool HasPermission(string permission);
        void Reset();
    }

    public class PermissionChecker : IPermissionChecker
    {
        private readonly AuthenticationStateProvider _authStateProvider;
        private readonly IApiClient _apiClient;
        private readonly SemaphoreSlim _initLock = new(1, 1);

        private bool _initialized;
        private HashSet<string> _allPerms = new();

        public PermissionChecker(AuthenticationStateProvider authStateProvider,IApiClient apiClient)
        {
            _authStateProvider = authStateProvider;
            _apiClient = apiClient;
        }

        public async Task InitializeAsync()
        {
            if (_initialized) return;
            await _initLock.WaitAsync();
            try
            {
                if (_initialized) return;

                var authState = await _authStateProvider.GetAuthenticationStateAsync();
                var user = authState.User;
                if (user?.Identity?.IsAuthenticated != true)
                {
                    _initialized = true;
                    return;
                }

                var roleIds = user.Claims
                    .Where(c => c.Type == "roleid")
                    .Select(c => int.TryParse(c.Value, out var id) ? id : 0)
                    .Where(id => id > 0)
                    .Distinct()
                    .ToArray();

                if (roleIds.Length == 0)
                {
                    _initialized = true;
                    return;
                }
                var responses = await Task.WhenAll(
                    roleIds.Select(id =>
                        _apiClient.GetFromJsonAsync<BaseResponseModel>(
                            $"api/permission/getListByRoleId/{id}"
                        )
                    )
                );
                var perms = responses
                    .Where(r => r != null && r.Success)
                    .SelectMany(r => JsonConvert
                        .DeserializeObject<List<PermissionModel>>(r.Data.ToString())!)
                    .Where(p => p.IsActive)
                    .Select(p => p.PermissionName);

                _allPerms = new HashSet<string>(perms);
                _initialized = true;
            }
            finally
            {
                _initLock.Release();
            }
        }

        public bool HasPermission(string permission)
            => _initialized && _allPerms.Contains(permission);
        public void Reset()      
        {
            _initLock.Wait();
            try
            {
                _initialized = false;
                _allPerms.Clear();
            }
            finally
            {
                _initLock.Release();
            }
        }
    }
}