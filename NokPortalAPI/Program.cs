using System.Data;
using System.Data.SqlClient;
using System.Text.Json.Serialization;
using FluentValidation;
using FluentValidation.AspNetCore;
using NokCore.Api.JwtToken.Services;
using NokPortal.Domains.Middlewares;
using NokPortal.Domains.Models;
using NokPortal.Domains.Repositories;
using NokPortal.Domains.Services;
using NokPortal.Domians.Models;
using NokPortal.Domians.Repositorys;
using NokPortal.Domians.Services;
using NokPortal.Domians.Validation;
using NokPortal.Shared.DB;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddScoped<IDbConnection>(sp =>
            new SqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));

        // DB
        builder.Services.AddScoped<DbConnectionFactory>();
        builder.Services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();

        // Repository
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IAppRepository, AppRepository>();
        builder.Services.AddScoped<IUserAppRepository, UserAppRepository>();
        builder.Services.AddScoped<IAppEnvRepository, AppEnvRepository>();

        // Service
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IAppService, AppService>();
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<IManagePayloadService, ManagePayloadService>();
        builder.Services.AddScoped<IAppEnvService, AppEnvService>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IUserAppsService, UserAppsService>();

        builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

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
        builder.Services.AddValidatorsFromAssemblyContaining<RequestApp>();
        builder.Services.AddValidatorsFromAssemblyContaining<RequestAppInfo>();
       // builder.Services.AddValidatorsFromAssemblyContaining<RequestCreateAppEnvValidator>();
        builder.Services.AddValidatorsFromAssemblyContaining<ModelUserApp>();
        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        builder.Services.AddControllers();
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

        app.UseMiddleware<JWTmiddleware>();

        // app.UseAuthentication();
        // app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}
