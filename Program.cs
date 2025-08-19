using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NokAir.Core.Interfaces.Rbac.Services;
using NokAir.Shared.Api.Responses.Factories;
using NokAir.Shared.Api.Responses.Factories.InHouse;
using NokAir.Shared.Middlewares.InHouse.Common;
using NokAir.Shared.Security.AuthorizationHandlers;
using NokAir.Shared.Security.Models.Common;
using NokAir.Shared.Security.Services.InHouse;
using NokPortalAPI.Dtos;
using NokPortalAPI.Entities;
using NokPortalAPI.Repositories;
using NokPortalAPI.Resources;
using NokPortalAPI.Services;
using NokPortalAPI.Shareds;
using NpgsqlTypes;
using Serilog;
using System.Globalization;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using NokAir.Configuration.Extensions;
using Serilog.Sinks.PostgreSQL;

namespace NokPortalAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Read environment variables
            var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production";
            var encryptionKey = Environment.GetEnvironmentVariable("ENCRYPTION_KEY")
                ?? throw new InvalidOperationException("ENCRYPTION_KEY environment variable is not set");
            var ivKey = Environment.GetEnvironmentVariable("IV_KEY")
                ?? throw new InvalidOperationException("IV_KEY environment variable is not set");

            // Get the host name
            var hostName = Dns.GetHostName();

            // PHASE 1: Bootstrap with minimal configuration
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .Enrich.WithProperty("HostName", hostName)
                .WriteTo.Console()
                .CreateBootstrapLogger(); // Special bootstrap logger that can be replaced later

            Log.Information("Starting NokAir Flight IROP Service in {Environment} environment", environment);


            // Load configuration from database
            var initConfig = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonEncryptionFile(
                    prefixName: "appsettings",
                    environment: environment,
                    key: encryptionKey,
                    iv: ivKey)
                .Build();

            if (initConfig == null)
            {
                Log.Fatal("Failed to load initial configuration. Ensure appsettings.{environment}.json file exists.");
                throw new InvalidOperationException("Initial configuration not found.");
            }

            // Phase 2: Load configuration from database
            var configBuilder = new ConfigurationBuilder()
                 .AddConfiguration(initConfig)
                 .AddPostgreSqlConfiguration(initConfig["ConnectionStrings:PORTAL_DB_CONNECTION"]);

            var configuration = configBuilder.Build();

            // Get Serilog connection string from configuration
            var logTableName = configuration["Serilog:WriteTo:0:Args:tableName"] ?? "portal_app_logs";
            var schemaName = configuration["Serilog:WriteTo:0:Args:schemaName"] ?? "public";
            var autoCreateTable = bool.TryParse(configuration["Serilog:WriteTo:0:Args:needAutoCreateTable"], out var needAutoCreateTable) && needAutoCreateTable;

            var protalDbConnection = configuration["ConnectionStrings:PORTAL_DB_CONNECTION"]
               ?? throw new InvalidOperationException("PORTAL_DB_CONNECTION environment variable is not set");
            var protalDb_Log_Connection = configuration["ConnectionStrings:PORTAL_LOG_DB_CONNECTION"]
                ?? throw new InvalidOperationException("PORTAL_LOG_DB_CONNECTION environment variable is not set");

            // Define custom columns using ColumnWriterBase
            var columnOptions = ColumnOptions.Default;
            columnOptions.Add("host_name", new SinglePropertyColumnWriter("HostName", PropertyWriteMethod.Raw, NpgsqlDbType.Varchar, null, 128));
            columnOptions.Add("environment", new SinglePropertyColumnWriter("Environment", PropertyWriteMethod.Raw, NpgsqlDbType.Varchar, null, 64));
            columnOptions.Remove("message_template"); // Remove message_template to avoid storing full message template
            columnOptions.Remove("log_event"); // Remove log_event to avoid storing full log event object

            // Reconfigure Serilog with PostgreSQL sink
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration) // Read log levels from config
                .Enrich.FromLogContext()
                .Enrich.WithProperty("HostName", hostName)
                .Enrich.WithProperty("Environment", environment)
                .WriteTo.Console()
                .WriteTo.PostgreSQL(
                    connectionString: protalDb_Log_Connection,
                    tableName: logTableName,
                    columnOptions: columnOptions,
                    needAutoCreateTable: autoCreateTable,
                    schemaName: schemaName)
                .CreateLogger();
            Serilog.Debugging.SelfLog.Enable(msg => Console.Error.WriteLine(msg));

            Log.Information("Application configuration loaded from database");

            var dbInitialize = bool.Parse(Environment.GetEnvironmentVariable("DB_INTIALIZE") ?? "false");
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration.SetBasePath(AppDomain.CurrentDomain.BaseDirectory);
            builder.Configuration.Sources.Clear();
            builder.Configuration.AddConfiguration(configuration);

            // Set up application configuration based on environment
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
                options.UseNpgsql(protalDbConnection));
            builder.Services.Configure<ServiceSettings>(builder.Configuration.GetSection("ServiceSettings"));
            builder.Services.Configure<JwtSettingsModel>(builder.Configuration.GetSection("JwtSettings"));

            // Register IHttpContextAccessor
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // Repositories
            builder.Services.AddScoped<IUserRepository<User>, UserRepository>();
            builder.Services.AddScoped<IRoleRepository<Role>, RoleRepository>();
            builder.Services.AddScoped<IUserAppRoleAssignmentRepository, UserAppRoleAssignmentRepository>();
            builder.Services.AddScoped<IAppRepository, AppRepository>();

            // Services
            builder.Services.AddSingleton<MsActiveDirectoryService>();
            builder.Services.AddScoped<IUserService<UserDto>, UserService>();
            builder.Services.AddScoped<IUserAppRoleAssignmentService, UserAppRoleAssignmentService>();
            builder.Services.AddScoped<IAppService, AppService>();
            builder.Services.AddScoped<IRoleService, RoleService>();
            builder.Services.AddSingleton<IJwtService, JwtService>();

            builder.Services.AddSingleton<ReloadFileConfig>();
            builder.Services.AddSingleton<CorsPolicyReloader>();

            builder.Services.AddScoped<IResponseFactory, ResponseFactory<ApiResponseLocalize>>();

            // Role service
            builder.Services.AddScoped<IAuthorizationHandler, RoleAuthorizationHandler>();
            builder.Services.AddScoped<IAuthorizationHandler, MultiRoleAuthorizationHandler>();
            builder.Services.AddScoped<IRoleServiceBase, RoleService>();

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
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
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
                    var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettingsModel>();
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings?.Issuer ?? "NokAir",
                        ValidAudience = jwtSettings?.Audience ?? "NokAir",
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings?.SecretKey ?? string.Empty))
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
            var permissions = new List<string> { "Root", "Admin", "EndUser" };

            // Register the permission handler.
            builder.Services.AddAuthorization(options =>
            {
                // Iterate through the list of permissions and add them to the policy.
                foreach (var permission in permissions)
                {
                    options.AddPolicy(permission, policy => policy.Requirements.Add(new RoleRequirementModel(permission)));
                }
                options.AddPolicy("RootOnly", policy => policy.Requirements.Add(new MultiRoleRequirementModel(new[] { "Root", })));
                options.AddPolicy("RootOrAdmin", policy => policy.Requirements.Add(new MultiRoleRequirementModel(new[] { "Root", "Admin" })));
                options.AddPolicy("AllRole", policy => policy.Requirements.Add(new MultiRoleRequirementModel(new[] { "Root", "Admin", "EndUser" })));
            });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            if (dbInitialize)
            {
                // Initialize the database with seed data
                using (var scope = app.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    var context = services.GetRequiredService<AppDbContext>();
                    DbInitializer.Initialize(context);
                }
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Register JWT middleware to check for JWT token in the request header.
            // For endpoints that not require JWT token, add [AllowAnonymous] attribute.
            app.UseMiddleware<JwtMiddleware>();

            app.UseHttpsRedirection();

            app.UseCors("AllowSpecificOrigin");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}