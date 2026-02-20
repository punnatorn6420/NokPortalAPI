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
    /// <summary>
    /// Controller for application management.
    /// </summary>
    [ApiController]
    [Route("v1/apps")]
    public class AppController : InHouseControllerBase
    {
        private readonly IAppService appService;
        private readonly IUserAppRoleAssignmentService userAppRoleAssignmentService;
        private readonly IUserService<UserDto> userService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppController"/> class.
        /// </summary>
        public AppController(
            IAppService appService,
            IUserAppRoleAssignmentService targetAppService,
            IUserService<UserDto> userService,
            IResponseFactory apiResponseFactory)
            : base(apiResponseFactory)
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
        public async Task<ActionResult> GetAppInfoAsync(int id)
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
        public async Task<ActionResult> GetRolesAsync(int id, [FromQuery] EnvironmentType env)
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

        /// <summary>
        /// This endpoint is used to get user roles by app.
        /// </summary>
        [HttpGet("{appId}/user/{userId}/roles")]
        [Authorize(Policy = "RootOrAdmin")]
        public async Task<ActionResult> GetUserRolesByAppAsync(int userId, int appId, [FromQuery] EnvironmentType env)
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
        public async Task<ActionResult> SearchAppAsync(
            [FromQuery] string? keyword = null,
            [FromQuery] int? pageNumber = 1,
            [FromQuery] int? pageSize = 25,
            [FromQuery] bool? ascending = true,
            [FromQuery] string? sortField = null,
            [FromQuery] bool? lightweight = false)
        {
            try
            {
                var criteria = new AppSearchDto
                {
                    Keyword = keyword ?? string.Empty,
                    SortField = sortField ?? string.Empty,
                    PageNumber = pageNumber ?? 1,
                    PageSize = pageSize ?? 25,
                    Ascending = ascending ?? true,
                };
                if (!ModelState.IsValid)
                {
                    return BadRequestResponseFromInvalidRequest();
                }

                var (items, total) = await appService.GetAppsByCriteriaAsync(criteria);

                object appItems;
                if (lightweight == true)
                {
                    appItems = items.Select(a => new
                    {
                        a.Id,
                        a.Name,
                    }).ToList();
                }
                else
                {
                    appItems = items;
                }

                return OkResponseWithResult(new
                {
                    totalRecords = total,
                    items = appItems
                });
            }
            catch (Exception ex)
            {
                return InternalServerErrorResponseFromException(ex);
            }
        }

        /// <summary>
        /// This endpoint is used to delete an app by id.
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Policy = "RootOrAdmin")]
        public async Task<ActionResult> DeleteAppAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestResponseFromInvalidRequest();
            }

            try
            {
                var isDeleted = await appService.DeleteAppByIdAsync(id);
                if (!isDeleted)
                {
                    return BadRequestResponseFromMessage("Application not found.");
                }

                return OkSuccessResponse();
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