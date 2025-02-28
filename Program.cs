using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NokCore.Api.JWT.Models;
using NokCore.Api.JWT.Services;
using NokCore.Api.Middlewares;
using NokCore.Api.Responses.Web;
using NokPortalAPI.Models;
using NokPortalAPI.Repositories;
using NokPortalAPI.Resources;
using NokPortalAPI.Services;
using NokPortalAPI.Shareds;
using Serilog;
using System.Globalization;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;

namespace NokPortalAPI
{
    public class Program
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

            // Add Localization and set resource path
            builder.Services.AddLocalization();
            builder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                    new CultureInfo("en-US"),
                };

                options.DefaultRequestCulture = new RequestCulture(culture: "en-US", uiCulture: "en-US");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;

                options.AddInitialRequestCultureProvider(new CustomRequestCultureProvider(async context =>
                {
                    return await Task.FromResult(new ProviderCultureResult("en"));
                }));
            });

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("NokPortalDB")));
            builder.Services.Configure<ServiceSettings>(builder.Configuration.GetSection("ServiceSettings"));
            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

            // Register IHttpContextAccessor
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // Repositories
            builder.Services.AddScoped<IUserRepository<User>, UserRepository>();
            builder.Services.AddScoped<IAppRepository, AppRepository>();
            builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();

            // Services
            builder.Services.AddSingleton<MsActiveDirectoryService>();
            builder.Services.AddScoped<IUserService<User>, UserService>();
            builder.Services.AddScoped<IAppService, AppService>();
            builder.Services.AddScoped<IPermissionService, PermissionService>();
            builder.Services.AddSingleton<IJwtService, JwtService>();

            builder.Services.AddSingleton<ReloadFileConfig>();
            builder.Services.AddSingleton<CorsPolicyReloader>();

            builder.Services.AddScoped<IApiResponseFactory, ApiResponseFactory<ApiResponseLocalize>>();

            // Permission service
            builder.Services.AddScoped<IAuthorizationHandler, NokCore.Api.Authorizations.PermissionHandler>();
            builder.Services.AddScoped<IAuthorizationHandler, NokCore.Api.Authorizations.MultiPermissionHandler>();
            builder.Services.AddScoped<NokCore.Identity.Services.IBasePermissionService, PermissionService>();

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

            //builder.Services.AddFluentValidationAutoValidation();
            //builder.Services.AddFluentValidationClientsideAdapters();
            //builder.Services.AddValidatorsFromAssemblyContaining<RequestTokenValidation>();

            //builder.Services.AddValidatorsFromAssemblyContaining<App>();

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                })
                .ConfigureApiBehaviorOptions(options =>
                {
                    // Disable the default model state validation filter.
                    options.SuppressModelStateInvalidFilter = true;
                });

            // Add JWT authentication
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                        ValidAudience = builder.Configuration["JwtSettings:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"] ?? string.Empty))
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                            logger.LogError(context.Exception, "Authentication failed.");
                            return Task.CompletedTask;
                        },
                        OnTokenValidated = context =>
                        {
                            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                            logger.LogInformation("Token validated successfully.");
                            return Task.CompletedTask;
                        },
                        OnChallenge = context =>
                        {
                            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                            logger.LogWarning(context.AuthenticateFailure, "Token validation challenge: {Message}", context.AuthenticateFailure?.Message);
                            return Task.CompletedTask;
                        }
                    };
                });

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