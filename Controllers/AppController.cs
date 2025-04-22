using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NokAir.Core.Exceptions;
using NokAir.Shared.Api.Responses.Factories;
using NokAir.Shared.Controllers;
using NokPortalAPI.Enums;
using NokPortalAPI.Models;
using NokPortalAPI.Services;

namespace NokPortalAPI.Controllers
{
    [ApiController]
    [Route("v1/apps")]
    public class AppController : InHouseControllerBase
    {
        private readonly IAppService appService;
        private readonly IUserAppRoleAssignmentService userAppRoleAssignmentService;
        private readonly IUserService<User> userService;

        public AppController(
            IAppService appService,
            IUserAppRoleAssignmentService targetAppService,
            IUserService<User> userService,
            IResponseFactory apiResponseFactory) : base(apiResponseFactory)
        {
            this.appService = appService;
            this.userAppRoleAssignmentService = targetAppService;
            this.userService = userService;
        }

        /// <summary>
        /// This endpoint is used to create a new app.
        /// </summary>
        [HttpPost("")]
        [Authorize(Policy = "RootOrAdmin")]
        public async Task<ActionResult> AddNewAppAsync(App app)
        {
            // Check if the model state is valid
            if (!ModelState.IsValid)
            {
                return BadRequestResponseFromInvalidRequest();
            }

            try
            {
                var addedApp = await appService.AddAppAsync(app);
                if (addedApp != null)
                {
                    return OkSuccessResponse();
                }

                return InternalServerErrorResponseFromException(new Exception("Failed to add new application"));
            }
            catch (DataValidationException ex)
            {
                return BadRequestResponseFromErrorCode(ex.ErrorCode);
            }
            catch (Exception ex)
            {
                return InternalServerErrorResponseFromException(ex);
            }
        }

        /// <summary>
        /// This endpoint is used to assign a user to an app.
        /// </summary>
        [HttpPost("{id}/assign-user-to-app")]
        [Authorize(Policy = "RootOrAdmin")]
        public async Task<ActionResult> AssignUserToAppAsync(int id, UserAppAssignmentRequest req)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestResponseFromInvalidRequest();
            }

            try
            {
                await userAppRoleAssignmentService.AssignUserToAppAsync(req);
                return OkSuccessResponse();
            }
            catch (DataValidationException ex)
            {
                return BadRequestResponseFromMessage(ex.Message);
            }
            catch (Exception ex)
            {
                return InternalServerErrorResponseFromException(ex);
            }
        }

        /// <summary>
        /// This endpoint is used to get the app info by app id.
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Policy = "RootOrAdmin")]
        public async Task<ActionResult> GetAppInfo(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestResponseFromInvalidRequest();
            }

            try
            {
                App? app = await appService.GetAppByIdAsync(id);
                if (app == null)
                {
                    return NoContent();
                }

                return OkResponseWithResult(app);
            }
            catch (Exception ex)
            {
                return InternalServerErrorResponseFromException(ex);
            }
        }

        /// <summary>
        /// This endpoint is used to get the list of roles for a specific app.
        /// </summary>
        [HttpGet("{id}/get-roles")]
        [Authorize(Policy = "RootOrAdmin")]
        public async Task<ActionResult> GetRoles(int id, [FromQuery] EnvironmentType env)
        {
            try
            {
                ICollection<Role> roles = await userAppRoleAssignmentService.GetRolesByAppIdAsync(id);
                return OkResponseWithResult(roles);
            }
            catch (Exception ex)
            {
                return InternalServerErrorResponseFromException(ex);
            }
        }

        /// <summary>
        /// This endpoint is used to get the JWT token to access the target app for Admin.
        /// </summary>
        [HttpGet("{id}/jwt-token")]
        [Authorize(Policy = "RootOrAdmin")]
        public async Task<ActionResult> GetJwtTokenInfoAsync(int id)
        {
            try
            {
                var userClaims = GetUserClaims();
                if (!userClaims.IsValid)
                {
                    return Unauthorized();
                }

                // Get app information
                var app = await appService.GetAppByIdAsync(id);
                if (app == null)
                {
                    throw new DataValidationException("Application not found. Please check the app.");
                }

                var jwtTokenInfo = await userAppRoleAssignmentService.GetJwtTokenInfoByUserAppAsync(userClaims.UserId, id);

                var res = new AppInfo()
                {
                    BaseUrl = app.BaseUrl,
                    EnvironmentType = app.EnvironmentType,
                    JwtToken = jwtTokenInfo.Token,
                    JwtExpiryTime = jwtTokenInfo.ExpiryTime
                };

                return OkResponseWithResult(res);
            }
            catch (DataValidationException ex)
            {
                return BadRequestResponseFromMessage(ex.Message);
            }
            catch (Exception ex)
            {
                return InternalServerErrorResponseFromException(ex);
            }
        }

        /// <summary>
        /// This endpoint is used to get all apps.
        /// </summary>
        [HttpGet("search")]
        [Authorize(Policy = "RootOrAdmin")]
        public async Task<ActionResult> SearchAppAsync([FromQuery] AppSearchCriteria searchCriteria)
        {
            try
            {
                // Validate search criteria
                if (!ModelState.IsValid)
                {
                    return BadRequestResponseFromInvalidRequest();
                }

                ICollection<App> apps = await appService.GetAppsByCriteriaAsync(searchCriteria);
                return OkResponseWithResult(apps);
            }
            catch (Exception ex)
            {
                return InternalServerErrorResponseFromException(ex);
            }
        }

        /// <summary>
        /// This endpoint is used to update an existing app.
        /// </summary>
        [HttpPut("")]
        [Authorize(Policy = "RootOrAdmin")]
        public async Task<ActionResult> UpdateAppAsync(App app)
        {
            // Check if the model state is valid
            if (!ModelState.IsValid)
            {
                return BadRequestResponseFromInvalidRequest();
            }

            try
            {
                await appService.UpdateAppAsync(app);
                return OkSuccessResponse();
            }
            catch (DataValidationException ex)
            {
                return BadRequestResponseFromErrorCode(ex.ErrorCode);
            }
            catch (Exception ex)
            {
                return InternalServerErrorResponseFromException(ex);
            }
        }
    }
}