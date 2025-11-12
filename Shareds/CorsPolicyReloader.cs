using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Options;

/// <summary>
/// Class for reloading CORS policy.
/// </summary>
public class CorsPolicyReloader
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="CorsPolicyReloader"/> class
    /// </summary>
    public CorsPolicyReloader(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Reloads the CORS policy based on the current configuration.
    /// </summary>
    public void ReloadCorsPolicy()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var corsOrigins = configuration.GetSection("CorsAllowedOrigins").Get<string[]>() ?? throw new ArgumentNullException("Cors:AllowedOrigins configuration is missing");

            var corsOptions = scope.ServiceProvider.GetRequiredService<IOptionsMonitor<CorsOptions>>();
            var options = corsOptions.CurrentValue;
            options.AddPolicy(
                "AllowSpecificOrigin",
                policyBuilder => policyBuilder.WithOrigins(corsOrigins)
                                              .AllowAnyHeader()
                                              .AllowAnyMethod());
        }
    }
}