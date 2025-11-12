using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NokAir.Core.Exceptions;
using NokAir.Shared.Api.Responses.Factories;
using NokAir.Shared.Controllers;
using NokPortalAPI.Dtos;
using NokPortalAPI.Entities;
using NokPortalAPI.Services;

namespace NokPortalAPI.Controllers
{
    /// <summary>
    /// Controller for user-related operations.
    /// </summary>
    [ApiController]
    [Route("v1/me")]
    public class MeController : InHouseControllerBase
    {
        private readonly IAppService appService;
        private readonly IUserAppRoleAssignmentService userAppRoleAssignmentService;
        private readonly IUserService<UserDto> userService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MeController"/> class.
        /// </summary>
        public MeController(
            IAppService appService,
            IUserAppRoleAssignmentService userAppRoleAssignmentService,
            IUserService<UserDto> userService,
            IResponseFactory resFactory)
            : base(resFactory)
        {
            this.appService = appService;
            this.userAppRoleAssignmentService = userAppRoleAssignmentService;
            this.userService = userService;
        }

        /// <summary>
        /// Get user information based on the JWT token.
        /// </summary>
        [HttpGet("")]
        [Authorize(Policy = "AllRole")]
        public async Task<ActionResult> GetMeInfo()
        {
            try
            {
                // Get user claims from http context.
                var userClaims = GetUserClaims();
                if (!userClaims.IsValid)
                {
                    return Unauthorized("User claims not found");
                }

                MyProfileDto? user = await userService.GetMyProfileAsync(userClaims.UserId);
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
        [Authorize(Policy = "AllRole")]
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
                    ClientUrl = app.ClientUrl,
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