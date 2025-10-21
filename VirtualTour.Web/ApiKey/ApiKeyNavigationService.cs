using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.WebUtilities;

namespace VirtualTour.Web.ApiKey
{
    public class ApiKeyNavigationService : IDisposable
    {
        private readonly NavigationManager _navigationManager;
        private string _apiKey = string.Empty;
        public string CurrentApiKey => _apiKey;
        public ApiKeyNavigationService(NavigationManager navigationManager)
        {
            _navigationManager = navigationManager;
            // Subscribe to navigation events
            _navigationManager.LocationChanged += OnLocationChanged;
        }

        private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
        {
            // Parse the current query parameters
            var uri = new Uri(e.Location);
            var queryParams = QueryHelpers.ParseQuery(uri.Query);

            // If the apiKey is missing, append it
            if (!_apiKey.Equals(string.Empty, StringComparison.Ordinal) &&
                !queryParams.ContainsKey("apiKey"))
            {
                var baseUri = uri.GetLeftPart(UriPartial.Path);
                var newUrl = QueryHelpers.AddQueryString(baseUri, "apiKey", _apiKey);
                // Navigate to the URL with the apiKey without forcing a full page reload.
                _navigationManager.NavigateTo(newUrl, forceLoad: false);
            }
        }
        public void RemoveApiKey()
        {
            _apiKey = string.Empty;
            var uri = new Uri(_navigationManager.Uri);
            var baseUri = uri.GetLeftPart(UriPartial.Path);
            var queryParams = QueryHelpers.ParseQuery(uri.Query);
            // Create a dictionary without the "apiKey" parameter.
            var newQueryDictionary = queryParams.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToString());
            newQueryDictionary.Remove("apiKey");
            var newUrl = newQueryDictionary.Any()
                ? QueryHelpers.AddQueryString(baseUri, newQueryDictionary)
                : baseUri;
            _navigationManager.NavigateTo(newUrl, forceLoad: false);
        }
        // Call this method on successful login to set the api key
        public void SetApiKey(string apiKey)
        {
            _apiKey = apiKey;
        }

        public void Dispose()
        {
            _navigationManager.LocationChanged -= OnLocationChanged;
        }
    }
}
