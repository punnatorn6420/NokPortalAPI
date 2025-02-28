using Microsoft.AspNetCore.Mvc;
using NokCore.Api.Controllers.Internal;
using NokCore.Api.Responses.Web;
using NokCore.Exceptions;
using NokPortalAPI.Enums;
using NokPortalAPI.Models;
using NokPortalAPI.Services;

namespace NokPortalAPI.Controllers
{
    [ApiController]
    [Route("v1/me")]
    public class MeController : BaseController
    {
        private readonly IAppService appService;
        private readonly IUserAppRoleAssignmentService userAppRoleAssignmentService;
        private readonly IUserService<User> userService;

        public MeController(
            IAppService appService,
            IUserAppRoleAssignmentService userAppRoleAssignmentService,
            IUserService<User> userService,
            IApiResponseFactory apiResponseFactory) : base(apiResponseFactory)
        {
            this.appService = appService;
            this.userAppRoleAssignmentService = userAppRoleAssignmentService;
            this.userService = userService;
        }

        /// <summary>
        /// Get user information based on the JWT token.
        /// </summary>
        [HttpGet("")]
        public async Task<ActionResult> GetMeInfo([FromQuery] EnvironmentType env)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestResponseFromInvalidRequest();
            }

            try
            {
                // Get user claims from http context.
                var userClaims = GetUserClaims();
                if (!userClaims.IsValid)
                {
                    return Unauthorized("User claims not found");
                }

                User? user = await userService.GetUserByIdAsync(userClaims.UserId);
                if (user == null)
                {
                    return NotFound("User not found");
                }

                return OkResponseWithResult(user);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Response status code does not indicate success: 401 (Unauthorized).")
                {
                    return Unauthorized("Token is either not in correct format or has expired");
                }
                else
                {
                    return InternalServerErrorResponseFromException(ex);
                }
            }
        }

        /// <summary>
        /// This endpoint is used to get the JWT token for the target app.
        /// </summary>
        [HttpGet("get-jwt-token")]
        public async Task<ActionResult> GetJwtTokenInfoAsync([FromQuery] int appId)
        {
            try
            {
                var userClaims = GetUserClaims();
                if (!userClaims.IsValid)
                {
                    return Unauthorized();
                }

                // Get app information
                var app = await appService.GetAppByIdAsync(appId);
                if (app == null)
                {
                    throw new DataValidationException("Application not found. Please check the app.");
                }

                var jwtTokenInfo = await userAppRoleAssignmentService.GetJwtTokenInfoByUserAppAsync(userClaims.UserId, appId);

                var res = new AppInfo()
                {
                    BaseUrl = app.BaseUrl,
                    EnvironmentType = app.EnvironmentType,
                    JwtToken = jwtTokenInfo.Token,
                    JwtExpiryTime = jwtTokenInfo.ExpiryTime
                };

                return OkResponseWithResult(res);
            }
            catch (Exception ex)
            {
                return InternalServerErrorResponseFromException(ex);
            }
        }
    }
}