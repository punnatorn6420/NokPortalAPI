using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NokCore.Api.Controllers;
using NokCore.Api.JWT.Models;
using NokCore.Exceptions;
using NokPortalAPI.Models;
using NokPortalAPI.Services.Old;

namespace NokPortalAPI.Controllers
{
    [ApiController]
    [Route("ad")]
    public class AdController : NokController<ControllerBase>
    {
        private readonly ServiceSettings serviceSettings;
        private readonly IUserService userService;
        private readonly IUserAppsService userAppsService;

        public AdController(IOptions<ServiceSettings> options, IUserService userService, IUserAppsService userAppsService)
        {
            this.serviceSettings = options.Value;
            this.userService = userService;
            this.userAppsService = userAppsService;
        }

        /// <summary>
        /// This endpoint is used to request a link for signing up with Microsoft Graph.
        /// </summary>
        [HttpGet("authorization-link-signup")]
        [AllowAnonymous]
        public async Task<ActionResult> GetAuthorizationLinkSignupAsync()
        {
            try
            {
                var clientId = serviceSettings.OAuth2.ClientId ?? throw new InvalidConfigurationException("OAuth2 settings are missing");
                var responseType = serviceSettings.OAuth2.ResponseType ?? throw new InvalidConfigurationException("OAuth2 settings are missing");
                var redirectUri = serviceSettings.OAuth2.RedirectUriSignUp ?? throw new InvalidConfigurationException("OAuth2 settings are missing");
                var scope = serviceSettings.OAuth2.Scope ?? throw new InvalidConfigurationException("OAuth2 settings are missing");
                var state = serviceSettings.OAuth2.State ?? throw new InvalidConfigurationException("OAuth2 settings are missing");
                var tenantId = serviceSettings.OAuth2.TenantId ?? throw new InvalidConfigurationException("OAuth2 settings are missing");
                var authorizationEndpoint = $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/authorize";
                var authorizationUrl = $"{authorizationEndpoint}?client_id={Uri.EscapeDataString(clientId)}&response_type={Uri.EscapeDataString(responseType)}&redirect_uri={Uri.EscapeDataString(redirectUri)}&scope={Uri.EscapeDataString(scope)}&state={Uri.EscapeDataString(state)}";

                return await Task.FromResult(Ok(FormatSuccessResponse(new { link = authorizationUrl })));
            }
            catch (Exception ex)
            {
                return Ok(FormatInternalErrorReponse(ex.Message, null));
            }
        }

        /// <summary>
        /// This endpoint is used to sign up with Microsoft Graph.
        /// </summary>
        [HttpPost("signup")]
        [AllowAnonymous]
        public async Task<ActionResult> SignupAsync(RequestMicrosoftToken req)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(FormatInvalidFieldResponse(GetFieldErrors()));
                }

                await userService.SignupMicrosoftGraphGetMeAsync(req.Token);

                return Ok(FormatSuccessResponse("Success"));
            }
            catch (DataValidationException ex)
            {
                return Ok(FormatInternalErrorReponse(ex.Message, null));
            }
            catch (Exception ex)
            {
                if (ex.Message == "Response status code does not indicate success: 401 (Unauthorized).")
                {
                    return Ok(FormatInternalErrorReponse("Token is either not in correct format or has expired", null));
                }
                else
                {
                    return Ok(FormatInternalErrorReponse(ex.Message, null));
                }
            }
        }

        /// <summary>
        /// This endpoint is used to request a link for signing in with Microsoft Graph.
        /// </summary>
        [HttpGet("authorization-link-signin")]
        [AllowAnonymous]
        public async Task<ActionResult> GetAuthorizationLinkSignInAsync()
        {
            try
            {
                var clientId = serviceSettings.OAuth2.ClientId ?? throw new InvalidConfigurationException("OAuth2 settings are missing");
                var responseType = serviceSettings.OAuth2.ResponseType ?? throw new InvalidConfigurationException("OAuth2 settings are missing");
                var redirectUri = serviceSettings.OAuth2.RedirectUriSignIn ?? throw new InvalidConfigurationException("OAuth2 settings are missing");
                var scope = serviceSettings.OAuth2.Scope ?? throw new InvalidConfigurationException("OAuth2 settings are missing");
                var state = serviceSettings.OAuth2.State ?? throw new InvalidConfigurationException("OAuth2 settings are missing");
                var tenantId = serviceSettings.OAuth2.TenantId ?? throw new InvalidConfigurationException("OAuth2 settings are missing");
                var authorizationEndpoint = $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/authorize";
                var authorizationUrl = $"{authorizationEndpoint}?client_id={Uri.EscapeDataString(clientId)}&response_type={Uri.EscapeDataString(responseType)}&redirect_uri={Uri.EscapeDataString(redirectUri)}&scope={Uri.EscapeDataString(scope)}&state={Uri.EscapeDataString(state)}";

                return await Task.FromResult(Ok(FormatSuccessResponse(new { link = authorizationUrl })));
            }
            catch (Exception ex)
            {
                return Ok(FormatInternalErrorReponse(ex.Message, null));
            }
        }

        /// <summary>
        /// This endpoint is used to sign in with Microsoft Graph.
        /// </summary>
        [HttpPost("signin")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<object, string>>> SiginAsync(RequestMicrosoftToken req)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(FormatInvalidFieldResponse(GetFieldErrors()));
            }

            try
            {
                JwtResponse jwt_token = await userService.SigninMicrosoftGraphGetMeAsync(req.Token);

                return Ok(FormatSuccessResponse(jwt_token));
            }
            catch (Exception ex)
            {
                if (ex.Message == "Response status code does not indicate success: 401 (Unauthorized).")
                {
                    return Unauthorized(FormatInternalErrorReponse("Token is either not in correct format or has expired", null));
                }
                else
                {
                    return Ok(FormatInternalErrorReponse(ex.Message, null));
                }
            }
        }
    }
}
