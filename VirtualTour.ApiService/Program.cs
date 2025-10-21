using VirtualTour.ApiService.Authorization;
using VirtualTour.ApiService.Data;
using VirtualTour.ApiService.Middleware;
using VirtualTour.BL.Repositories;
using VirtualTour.BL.Services;
using VirtualTour.DataAccess;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Add service defaults & Aspire client in
// tegrations.
// Register services

builder.AddServiceDefaults();
builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddProblemDetails();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IDbContext, SqlDbContext>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
builder.Services.AddScoped<IPermissionChecker, PermissionChecker>();
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<IQRCodeRepository, QRCodeRepository>();
builder.Services.AddScoped<IQRCodeService, QRCodeService>();
builder.Services.AddScoped<INodeRepository, NodeRepository>();
builder.Services.AddScoped<INodeService, NodeService>();
builder.Services.AddScoped<IApiKeyRepository, ApiKeyRepository>();
builder.Services.AddScoped<IApiKeyService,ApiKeyService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();

// Authentication & Authorization
builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme =
    options.DefaultChallengeScheme =
    options.DefaultForbidScheme =
    options.DefaultScheme =
    options.DefaultSignInScheme =
    options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
}
    ).AddCookie(IdentityConstants.ApplicationScheme).AddJwtBearer(options =>
    {
        options.IncludeErrorDetails = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var authHeader = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    context.Fail("No valid authorization header found");
                    return;
                }
                var token = authHeader.Substring("Bearer ".Length).Trim();
                JwtSecurityToken jwtToken;
                try
                {
                    jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
                }
                catch (Exception ex)
                {
                    context.Fail($"Token parsing failed: {ex.Message}");
                    return;
                }

                var userRepository = context.HttpContext.RequestServices.GetRequiredService<IUserRepository>();
                bool tokenValid = await userRepository.IsTokenValidAsync(token);
                if (!tokenValid)
                {
                    context.Fail("Token is not valid");
                }
            },

        };
    });


builder.Services.AddAuthorization();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();

// Add services to the container.
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSwaggerGen();

var app = builder.Build();
// Seed Permission Data (chạy 1 lần khi start app)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedData.InitializePermissionsAsync(services);
}
// Configure the HTTP request pipeline.
app.UseExceptionHandler();

// Swagger chỉ bật ở môi trường Development | Production
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    // app.UseSwagger();
    //app.UseSwaggerUI();

}

// Swagger cấu hình luôn bật
app.UseSwagger(c =>
{
    c.RouteTemplate = "swagger/{documentName}/swagger.json";
});
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "VirtualTour API V1");
    c.RoutePrefix = "swagger"; // Swagger UI tại /swagger
});

// Điều hướng "/" về Swagger
app.MapGet("/", context =>
{
    context.Response.Redirect("/swagger");
    return Task.CompletedTask;
});

app.UseMiddleware<ApiKeyAuthenticationMiddleware>();
//app.UseHttpsRedirection();
app.UseCors(ops => ops.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapDefaultEndpoints();

app.Run();

