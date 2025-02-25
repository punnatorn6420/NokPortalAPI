namespace NokPortalAPI
{
    using FluentValidation;
    using FluentValidation.AspNetCore;
    using Microsoft.EntityFrameworkCore;
    using NokCore.Api.JWT.Services;
    using NokCore.Api.Middlewares;
    using NokPortalAPI.Models;
    using NokPortalAPI.Models.Validation;
    using NokPortalAPI.Repositories;
    using NokPortalAPI.Responses;
    using NokPortalAPI.Services;
    using NokPortalAPI.Shareds;
    using NokPortalAPI.Shareds.DB;
    using Serilog;
    using System.Net;
    using System.Text.Json.Serialization;

    public static class Program
    {
        public static void Main(string[] args)
        {
            // Build a base configuration to read the environment setting
            var baseConfig = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            // Read the environment setting from base configuration
            var environment = baseConfig["Environment"] ?? "Production";

            // Get the host name
            var hostName = Dns.GetHostName();

            // Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
                    .Build())
                .Enrich.FromLogContext()
                .Enrich.WithProperty("HostName", hostName)
                .CreateLogger();


            var builder = WebApplication.CreateBuilder(args);

            // Set up application configuration based on environment
            builder.Configuration.SetBasePath(AppDomain.CurrentDomain.BaseDirectory);
            builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                                 .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true);
            builder.Host.UseSerilog();

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("NokPortalDB")));
            builder.Services.Configure<ServiceSettings>(builder.Configuration.GetSection("ServiceSettings"));


            // Database factory
            builder.Services.AddSingleton<DbConnectionFactory>();

            // Repositories
            builder.Services.AddScoped<IUserRepository<User>, UserRepository>();
            builder.Services.AddScoped<IAppRepository, AppRepository>();

            // Services
            builder.Services.AddSingleton<MsActiveDirectoryService>();
            builder.Services.AddScoped<IUserService<User>, UserService>();
            builder.Services.AddScoped<IAppService, AppService>();

            builder.Services.AddSingleton<ReloadFileConfig>();
            builder.Services.AddSingleton<CorsPolicyReloader>();

            builder.Services.AddScoped<ApiResponseFactory>();

            // Permission service
            // builder.Services.AddSingleton<IAuthorizationHandler, NokCore.Api.Authorizations.PermissionHandler>();
            // builder.Services.AddSingleton<IAuthorizationHandler, NokCore.Api.Authorizations.MultiPermissionHandler>();


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
}