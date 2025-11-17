using Blazored.LocalStorage;
using VirtualTour.Model;
using VirtualTour.Web.ApiKey;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace VirtualTour.Web.Authentication
{
    public class ApiAuthenticationStateProvider: AuthenticationStateProvider
    {
       
        private readonly ILocalStorageService _localStorage;
        private readonly HttpClient _httpClient;
        private readonly ApiKeyNavigationService _apiKeyNavigationService;
        private readonly NavigationManager _navigationManager;
        public ApiAuthenticationStateProvider( ILocalStorageService localStorage, HttpClient httpClient, 
            ApiKeyNavigationService apiKeyNavigationService, NavigationManager navigationManager)
        {
            _localStorage = localStorage;
            _httpClient = httpClient;
            _apiKeyNavigationService = apiKeyNavigationService;
            _navigationManager = navigationManager;
        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var savedToken = await _localStorage.GetItemAsync<string>("authToken");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", savedToken);
                var tenantId = await _localStorage.GetItemAsync<string>("tenantId");
                if (!string.IsNullOrWhiteSpace(tenantId))
                {
                    if (_httpClient.DefaultRequestHeaders.Contains("X-Tenant-ID"))
                        _httpClient.DefaultRequestHeaders.Remove("X-Tenant-ID");
                    _httpClient.DefaultRequestHeaders.Add("X-Tenant-ID", tenantId);
                }
                var isTokenValid = await _httpClient.GetFromJsonAsync<bool>("api/auth/check-token");
                if (!isTokenValid || string.IsNullOrWhiteSpace(savedToken))
                {
                    await MarkUserAsLoggedOut();
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                }
                //var savedApiKey = await _localStorage.GetItemAsync<string>("apiKey");
                //if (!string.IsNullOrWhiteSpace(savedApiKey) && string.IsNullOrWhiteSpace(_apiKeyNavigationService.CurrentApiKey))
                //{
                //    _apiKeyNavigationService.SetApiKey(savedApiKey);
                //    _navigationManager.NavigateTo(_navigationManager.Uri);
                //}
                var identity = new ClaimsIdentity();
                identity = GetClaimsIdentity(savedToken);
                return new AuthenticationState(new ClaimsPrincipal(identity));
            }
            catch (Exception ex)
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
        }

        public async Task MarkUserAsAuthenticated(LoginResponse model)
        {
            await _localStorage.SetItemAsync("authToken", model.Token);
            //await _localStorage.SetItemAsync("apiKey", model.ApiKey);
            await _localStorage.SetItemAsync("tenantId", model.TenantId);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", model.Token);
            //_apiKeyNavigationService.SetApiKey(model.ApiKey);
            var identity = GetClaimsIdentity(model.Token);
            if (!identity.HasClaim(c => c.Type == ClaimTypes.Name))
            {
                identity.AddClaim(new Claim(ClaimTypes.Name, model.UserName));
            }
            var authenticatedUser = new ClaimsPrincipal(identity);
            var authState = new AuthenticationState(authenticatedUser);
            NotifyAuthenticationStateChanged(Task.FromResult(authState));
        }

        public async Task MarkUserAsLoggedOut()
        {
            await _localStorage.RemoveItemAsync("authToken");
            //await _localStorage.RemoveItemAsync("apiKey");
            await _localStorage.RemoveItemAsync("tenantId");
            //_apiKeyNavigationService.RemoveApiKey();
            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
            var authState = Task.FromResult(new AuthenticationState(anonymousUser));
            NotifyAuthenticationStateChanged(authState);
        }
        private ClaimsIdentity GetClaimsIdentity(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var claims = jwtToken.Claims;
            return new ClaimsIdentity(claims, "jwt", "unique_name", ClaimTypes.Role);
        }
    }
}
