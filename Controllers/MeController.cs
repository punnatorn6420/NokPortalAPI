using Microsoft.AspNetCore.Mvc;
using NokCore.Api.Controllers;
using NokPortalAPI.Models;
using NokPortalAPI.Services.Old;

namespace NokPortalAPI.Controllers
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
            if (!ModelState.IsValid)
            {
                return BadRequest(FormatInvalidFieldResponse(GetFieldErrors()));
            }

            try
            {
                // Get user claims from http context.
                var userClaims = GetUserClaims();
                if (!userClaims.IsValid)
                {
                    return Unauthorized(FormatInternalErrorReponse("User claims not found", null));
                }

                UserApps user = await userService.GetUserAppsAsync(userClaims.UserId, env);
                return Ok(FormatSuccessResponse(user));
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