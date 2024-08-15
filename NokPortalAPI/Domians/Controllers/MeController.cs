using Microsoft.AspNetCore.Mvc;
using NokCore.Api.Controllers;
using NokCore.Api.JWT.Models;
using NokCore.Api.JWT.Services;
using NokPortalAPI.Domains.Models;
using NokPortalAPI.Domains.Services;

namespace NokPortalAPI.Domains.Controllers
{
    [ApiController]
    [Route("me")]
    public class MeController : NokController<ControllerBase>
    {
        private readonly IUserAppsService userAppsService;
        private readonly IUserService userService;

        public MeController(IUserAppsService userAppsService, IUserService userService)
        {
            this.userAppsService = userAppsService;
            this.userService = userService;
        }

        /// <summary>
        /// Get user information based on the JWT token.
        /// </summary>
        [HttpGet("")]
        public async Task<ActionResult> GetMeInfo([FromQuery] EnumEnvironmentType env)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.FormatInvalidFieldResponse(this.GetFieldErrors()));
            }

            try
            {
                // Get user claims from http context.
                var userClaims = this.GetUserClaims();
                if (!userClaims.IsValid)
                {
                    return this.Unauthorized(this.FormatInternalErrorReponse("User claims not found", null));
                }

                UserApps user = await this.userService.GetUserAppsAsync(userClaims.UserId, env);
                return this.Ok(this.FormatSuccessResponse(user));
            }
            catch (Exception ex)
            {
                if (ex.Message == "Response status code does not indicate success: 401 (Unauthorized).")
                {
                    return this.Unauthorized(this.FormatInternalErrorReponse("Token is either not in correct format or has expired", null));
                }
                else
                {
                    return this.Ok(this.FormatInternalErrorReponse(ex.Message, null));
                }
            }
        }
    }
}