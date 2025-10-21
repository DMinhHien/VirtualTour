using VirtualTour.BL.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace VirtualTour.ApiService.Middleware
{
    public class ApiKeyAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        public ApiKeyAuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            var authorizeData = endpoint?.Metadata.GetMetadata<IAuthorizeData>();

            if (authorizeData != null)
            {
                if (context.Request.Query.TryGetValue("apiKey", out var providedApiKey))
                {
                    var apiKeyService = context.RequestServices.GetRequiredService<IApiKeyService>();
                    if (await apiKeyService.ValidateApiKeyAsync(providedApiKey))
                    {
                        var claims = new List<Claim>
                        {
                            new Claim("apiKey", providedApiKey)
                        };
                        var identity = new ClaimsIdentity(claims, "ApiKey");
                        context.User = new ClaimsPrincipal(identity);
                    }
                    else
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        return;
                    }
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return;
                }
            }
            await _next(context);
        }
    }
}
