using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using NokCore.Api.Controllers;
using NokPortal.Domians.Services;

namespace NokPortal.Domains.Middlewares
{
    public class JWTmiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<JWTmiddleware> _logger;

        public JWTmiddleware(RequestDelegate next, ILogger<JWTmiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var jwtService = context.RequestServices.GetService<IJwtService>();

            if (jwtService == null)
            {
                throw new InvalidOperationException("IJwtService is not registered in the dependency injection container.");
            }

            ClaimsPrincipal? decodeJWT;

            var endpoint = context.GetEndpoint();
            if (endpoint?.Metadata.GetMetadata<IAllowAnonymous>() != null)
            {
                await _next(context);
                return;
            }

            if (!context.Request.Headers.ContainsKey("Authorization"))
            {
                if (!context.Response.HasStarted)
                {
                    context.Response.StatusCode = 401;

                    var response = new ApiResponse<object, string>
                    {
                        Status = "Error",
                        Data = null,
                        Error = new ApiError<string>
                        {
                            Type = "ValidationError",
                            UserMessage = "Authorization header missing",
                            DeveloperMessage = null
                        }
                    };

                    await context.Response.WriteAsJsonAsync(response);
                }
                return;
            }
            else
            {
                var authorizationHeader = context.Request.Headers["Authorization"].FirstOrDefault();
                decodeJWT = jwtService.DecodeToken(authorizationHeader ?? string.Empty);
                if (decodeJWT == null)
                {
                    context.Response.StatusCode = 401;

                    var response = new ApiResponse<object, string>
                    {
                        Status = "Error",
                        Data = null,
                        Error = new ApiError<string>
                        {
                            Type = "AuthenticationError",
                            UserMessage = "Token is either not in correct format or has expired",
                            DeveloperMessage = null
                        }
                    };

                    await context.Response.WriteAsJsonAsync(response);
                    return;
                }
            }

            if (decodeJWT != null)
            {
                foreach (var claim in decodeJWT.Claims)
                {
                    context.Items[claim.Type] = claim.Value;
                }
            }

            await _next(context);
        }
    }
}