using System.Text.Json.Serialization;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using NokCore.Api.JWT.Services;
using NokCore.Api.Middlewares;
using NokPortalAPI.Domains.Models;
using NokPortalAPI.Domains.Repositorys;
using NokPortalAPI.Domains.Services;
using NokPortalAPI.Domains.Validation;
using NokPortalAPI.Shared.DB;
using NokPortalAPI.Shareds;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // builder.Services.AddScoped<IDbConnection>(sp =>
        //     new SqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));

        // TODO: Check with team, the reason for use this approach
        builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

        // Database factory
        builder.Services.AddSingleton<DbConnectionFactory>();

        // Repositories
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IAppRepository, AppRepository>();
        builder.Services.AddScoped<IAssignedUsersRepositorys, AssignedUsersRepositorys>();
        builder.Services.AddScoped<IAppEnvRepository, AppEnvRepository>();
        builder.Services.AddScoped<IAppsRolesRepositorys, AppsRolesRepository>();

        // Services
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IAppService, AppService>();
        builder.Services.AddScoped<IAppEnvService, AppEnvService>();
        builder.Services.AddScoped<IUserAppsService, UserAppsService>();

        builder.Services.AddSingleton<ReloadFileConfig>();
        builder.Services.AddSingleton<CorsPolicyReloader>();

        // Permission service
        builder.Services.AddSingleton<IAuthorizationHandler, NokCore.Api.Authorizations.PermissionHandler>();
        builder.Services.AddSingleton<IAuthorizationHandler, NokCore.Api.Authorizations.MultiPermissionHandler>();
        builder.Services.AddSingleton<NokCore.Identity.Services.IPermissionService, NokCore.Identity.Services.PermissionService>();

        string secretKey = "secret123456789abcdefghigklmnopqrst";
        int hourExpire = 24;
        builder.Services.AddSingleton<IJwtService>(new JwtService(secretKey, hourExpire));

        // Add HttpClient
        builder.Services.AddHttpClient();

        // Add CORS policy
        var corsOrigins = builder.Configuration.GetSection("CorsAllowedOrigins").Get<string[]>() ?? throw new ArgumentNullException("Cors:AllowedOrigins configuration is missing");
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(
                "AllowSpecificOrigin",
                policyBuilder => policyBuilder.WithOrigins(corsOrigins)
                                              .AllowAnyHeader()
                                              .AllowAnyMethod());
        });

        builder.Services.AddFluentValidationAutoValidation();
        builder.Services.AddFluentValidationClientsideAdapters();
        builder.Services.AddValidatorsFromAssemblyContaining<RequestTokenValidation>();

        // builder.Services.AddValidatorsFromAssemblyContaining<RequestToken>();
        builder.Services.AddValidatorsFromAssemblyContaining<App>();
        builder.Services.AddValidatorsFromAssemblyContaining<RequestAppInfo>();

        // builder.Services.AddValidatorsFromAssemblyContaining<RequestCreateAppEnvValidator>();
        builder.Services.AddValidatorsFromAssemblyContaining<ModelUserApp>();
        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        builder.Services.AddControllers();

        // Set the list of permissions that will be used in the application.
        var permissions = new List<string> { "Admin", "EndUser" };

        // Register the permission handler.
        builder.Services.AddAuthorization(options =>
        {
            foreach (var permission in permissions)
            {
                options.AddPolicy(permission, policy => policy.Requirements.Add(new NokCore.Api.Authorizations.PermissionRequirement(permission)));
            }

            options.AddPolicy("CombinedPolicy", policy => policy.Requirements.Add(new NokCore.Api.Authorizations.MultiPermissionRequirement(new[] { "Admin", "EndUser" })));
        });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Configuration.SetBasePath(AppDomain.CurrentDomain.BaseDirectory).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseCors("AllowSpecificOrigin");

        app.UseAuthentication();
        app.UseAuthorization();

        // Register JWT middleware to check for JWT token in the request header.
        // For endpoints that not require JWT token, add [AllowAnonymous] attribute.
        app.UseMiddleware<JWTmiddleware>();

        app.MapControllers();

        app.Run();
    }
}
