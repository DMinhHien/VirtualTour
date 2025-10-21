using Blazored.LocalStorage;
using VirtualTour.Model;
using VirtualTour.Web.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using System.Globalization;
using System.Net.Http.Headers;

namespace VirtualTour.Web
{
    public interface IApiClient
    {
        Task SetAuthorizeHeader();
        Task<T> GetFromJsonAsync<T>(string path);
        Task<TResponse> PostAsync<TResponse, TRequest>(string path, TRequest model);
        Task<TResponse> PutAsync<TResponse, TRequest>(string path, TRequest model);
        Task<T> DeleteAsync<T>(string path);
    }
    public class ApiClient: IApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private readonly NavigationManager _navigationManager;
        private readonly ApiAuthenticationStateProvider _authStateProvider;
        private const string AuthTokenKey = "authToken";

        public ApiClient(HttpClient httpClient, ILocalStorageService localStorage, NavigationManager navigationManager, ApiAuthenticationStateProvider authStateProvider)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
            _navigationManager = navigationManager;
            _authStateProvider = authStateProvider;
        }

        public async Task SetAuthorizeHeader()
        {
            try
            {
                var currentUri = new Uri(_navigationManager.Uri);
                if (!currentUri.AbsolutePath.Contains("/admin", StringComparison.OrdinalIgnoreCase) ||
                    currentUri.AbsolutePath.Equals("/admin/login", StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }
                var token = await _localStorage.GetItemAsync<string>(AuthTokenKey);
                if (!string.IsNullOrEmpty(token) )
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    var isValid = await _httpClient.GetFromJsonAsync<bool>("api/auth/check-token");
                    if (!isValid)
                    {
                        _authStateProvider.MarkUserAsLoggedOut();
                        _navigationManager.NavigateTo("/admin/login", forceLoad: true);
                        return;
                    }
                    // Add culture cookie header
                    var requestCulture = new RequestCulture(CultureInfo.CurrentCulture, CultureInfo.CurrentUICulture);
                    var cultureCookieValue = CookieRequestCultureProvider.MakeCookieValue(requestCulture);

                    if (_httpClient.DefaultRequestHeaders.Contains("Cookie"))
                    {
                        _httpClient.DefaultRequestHeaders.Remove("Cookie");
                    }
                    _httpClient.DefaultRequestHeaders.Add("Cookie", $"{CookieRequestCultureProvider.DefaultCookieName}={cultureCookieValue}");
                }
                else
                {
                    _authStateProvider.MarkUserAsLoggedOut();
                    _navigationManager.NavigateTo("/admin/login", forceLoad: true);
                }
            }
            catch (Exception)
            {
                _navigationManager.NavigateTo("/admin/login", forceLoad: true);
            }
        }

        public async Task<T> GetFromJsonAsync<T>(string path)
        {
            await SetAuthorizeHeader();
            SetApiKey(ref path);
            var response = await _httpClient.GetAsync(path);
            if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                _navigationManager.NavigateTo("/unauthorized", forceLoad: true);
                return default;
            }

            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(json);
        }

        public async Task<TResponse> PostAsync<TResponse, TRequest>(string path, TRequest model)
        {
            await SetAuthorizeHeader();
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1));
            HttpResponseMessage response;
            try
            {
                SetApiKey(ref path);
                response = await _httpClient.PostAsJsonAsync(path, model, cts.Token);
            }
            catch (TaskCanceledException)
            {
                // Request timed out or was canceled
                return default;
            }
            if (response != null && response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<TResponse>(json);
            }
            return default;
        }

        public async Task<TResponse> PutAsync<TResponse, TRequest>(string path, TRequest model)
        {
            await SetAuthorizeHeader();
            SetApiKey(ref path);
            var response = await _httpClient.PutAsJsonAsync(path, model);
            if (response != null && response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<TResponse>(json);
            }
            return default;
        }

        public async Task<T> DeleteAsync<T>(string path)
        {
            await SetAuthorizeHeader();
            SetApiKey(ref path);
            return await _httpClient.DeleteFromJsonAsync<T>(path);
        }
        public void SetApiKey(ref string path)
        {
            var currentUri = new Uri(_navigationManager.Uri);
            var currentQuery = QueryHelpers.ParseQuery(currentUri.Query);
            if (currentQuery.TryGetValue("apiKey", out var apiKeyValue))
            {
                // Optionally, check if the provided path already has an apiKey parameter.
                var requestUri = QueryHelpers.AddQueryString(path, "dummy", "dummy"); // Create a dummy query to parse parameters.
                var pathUri = new Uri(_httpClient.BaseAddress, requestUri);
                var pathQuery = QueryHelpers.ParseQuery(pathUri.Query);
                if (!pathQuery.ContainsKey("apiKey"))
                {
                    // Append the API key to the request URL.
                    path = QueryHelpers.AddQueryString(path, "apiKey", apiKeyValue);
                }
            }
        }
    }
}