using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NokAir.Core.Exceptions;
using NokAir.Shared.Api.Responses.Factories;
using NokAir.Shared.Controllers;
using NokPortalAPI.Dtos;
using NokPortalAPI.Enums;
using NokPortalAPI.Services;

namespace NokPortalAPI.Controllers
{
    [ApiController]
    [Route("v1/apps")]
    public class AppController : InHouseControllerBase
    {
        private readonly IAppService appService;
        private readonly IUserAppRoleAssignmentService userAppRoleAssignmentService;
        private readonly IUserService<UserDto> userService;

        public AppController(
            IAppService appService,
            IUserAppRoleAssignmentService targetAppService,
            IUserService<UserDto> userService,
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
        public async Task<ActionResult> AddNewAppAsync(CreateAppRequestDto request)
        {
            // Check if the model state is valid
            if (!ModelState.IsValid)
            {
                return BadRequestResponseFromInvalidRequest();
            }

            try
            {
                // var addedApp = await appService.AddAppAsync(app);
                var appDto = new AppDto
                {
                    Name = request.Name,
                    Header = request.Header,
                    Subheader = request.Subheader,
                    EnvironmentType = request.EnvironmentType,
                    ClientUrl = request.ClientUrl,
                    BackendUrl = request.BackendUrl,
                    ImageUrl = request.ImageUrl,
                    SecretKey = request.SecretKey,
                    JwtExpiryHours = request.JwtExpiryHours,
                    Remark = request.Remark,
                    CreatedAt = DateTime.Now,
                    ModifiedAt = DateTime.Now,
                    Active = true
                };
                if (appDto != null)
                {
                    var addedApp = await appService.AddAppAsync(appDto);
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
        [HttpPost("assign-user-to-app")]
        [Authorize(Policy = "RootOrAdmin")]
        public async Task<ActionResult> AssignUserToAppAsync(UserAppAssignmentRequest req)
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
                AppDto? app = await appService.GetAppByIdAsync(id);
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
                ICollection<RoleDto> roles = await userAppRoleAssignmentService.GetRolesByAppIdAsync(id);
                return OkResponseWithResult(roles);
            }
            catch (Exception ex)
            {
                return InternalServerErrorResponseFromException(ex);
            }
        }

        [HttpGet("{appId}/user/{userId}/roles")]
        [Authorize(Policy = "RootOrAdmin")]
        public async Task<ActionResult> GetUserRolesByApp(int userId, int appId, [FromQuery] EnvironmentType env)
        {
            try
            {
                var userAppRole = await userAppRoleAssignmentService.GetUserAppRoleAsync(userId, appId);
                return OkResponseWithResult(userAppRole);
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
                    ClientUrl = app.ClientUrl,
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
        public async Task<ActionResult> SearchAppAsync([FromQuery] AppSearchDto searchCriteria)
        {
            try
            {
                // Validate search criteria
                if (!ModelState.IsValid)
                {
                    return BadRequestResponseFromInvalidRequest();
                }

                ICollection<AppDto> apps = await appService.GetAppsByCriteriaAsync(searchCriteria);
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
        [HttpPut("{id}")]
        [Authorize(Policy = "RootOrAdmin")]
        public async Task<ActionResult> UpdateAppAsync(int id, [FromBody] AppDto app)
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