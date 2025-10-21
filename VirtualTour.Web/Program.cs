using Blazored.LocalStorage;
using Blazored.Toast;
using VirtualTour.BL.Services;
using VirtualTour.Web.ApiKey;
using VirtualTour.Web.Authentication;
using VirtualTour.Web.Authorization;
using VirtualTour.Web.Components;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor.Services;
using VirtualTour.Web;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/admin/login"; // URL to your login page.

    });
builder.Services.AddBlazorBootstrap();
builder.Services.AddBlazoredToast();
builder.Services.AddMudServices();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();
builder.Services.AddScoped<IApiClient, ApiClient>();
builder.Services.AddOutputCache();
builder.Services.AddSingleton<UserProfileStateService>();
builder.Services.AddScoped<ApiKeyNavigationService>();
builder.Services.AddHttpClient<WeatherApiClient>(client =>
    {
        // This URL uses "https+http://" to indicate HTTPS is preferred over HTTP.
        // Learn more about service discovery scheme resolution at https://aka.ms/dotnet/sdschemes.
        client.BaseAddress = new("https+http://apiservice");
    });
builder.Services.AddHttpClient<IApiClient, ApiClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["BackendApiUrl"]);
});
// VirtualTour.Web\Program.cs
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.Configuration["BackendApiUrl"])});
builder.Services.AddAuthorizationCore();
builder.Services.AddOptions();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<ApiAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<ApiAuthenticationStateProvider>());
builder.Services.AddScoped<IPermissionChecker, PermissionChecker>();
builder.Services.AddHttpContextAccessor();

// 🚀 Thêm cấu hình Blazor Server tối ưu cho IIS + giảm lỗi "Failed to rejoin"
builder.Services.AddServerSideBlazor()
    .AddHubOptions(options =>
    {
        options.ClientTimeoutInterval = TimeSpan.FromSeconds(300); // thời gian client chờ
        options.KeepAliveInterval = TimeSpan.FromSeconds(60);     // giữ kết nối liên tục
    })
    .AddCircuitOptions(options =>
    {
        options.DetailedErrors = true; // bật lỗi chi tiết khi debug
        options.DisconnectedCircuitRetentionPeriod = TimeSpan.FromMinutes(5); // giữ trạng thái 3 phút
        // Nếu muốn tắt hoàn toàn rejoin:
        // options.DisconnectedCircuitRetentionPeriod = TimeSpan.Zero;
    });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.UseOutputCache();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapDefaultEndpoints();

app.Run();
