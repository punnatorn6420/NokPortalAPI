using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NokCore.Api.Controllers;
using NokCore.Errors;
using NokCore.Exceptions;
using NokPortalAPI.Models;
using NokPortalAPI.Responses;
using NokPortalAPI.Services;

namespace NokPortalAPI.Controllers
{
    [ApiController]
    [Route("app")]
    public class AppController : ControllerBase
    {
        private readonly IAppService appService;
        private readonly ITargetAppService targetAppService;
        private readonly IUserService<User> userService;
        private readonly ApiResponseFactory resFactory;

        public AppController(
            IAppService appService,
            ITargetAppService targetAppService,
            IUserService<User> userService,
            ApiResponseFactory apiResponseFactory)
        {
            this.appService = appService;
            this.targetAppService = targetAppService;
            this.userService = userService;
            this.resFactory = apiResponseFactory;
        }

        /// <summary>
        /// This endpoint is used to create a new app.
        /// </summary>
        [HttpPost("")]
        [Authorize(Policy = "Admin")]
        public async Task<ActionResult> AddNewAppAsync(App app)
        {
            // Check if the model state is valid
            if (!ModelState.IsValid)
            {
                return BadRequest(resFactory.CreateErrorResponse("InvalidField", "", null));
            }

            try
            {
                var addedApp = await appService.AddAppAsync(app);
                if (addedApp != null)
                {
                    return Ok(resFactory.CreateSuccessResponse("Success", null));
                }

                return Ok(resFactory.CreateErrorResponse("Create failed", "", null));
            }
            catch (DataValidationException ex)
            {
                return Ok(resFactory.CreateErrorResponse(ex.ErrorCode, ex.Message, null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, resFactory.CreateInternalErrorResponse(ServiceError.UnexpectedErrorE999, ex.Message, null));
            }
        }

        /// <summary>
        /// This endpoint is used to update an existing app.
        /// </summary>
        [HttpPut("")]
        [Authorize(Policy = "Admin")]
        public async Task<ActionResult> UpdateAppAsync(App app)
        {
            // Check if the model state is valid
            if (!ModelState.IsValid)
            {
                return BadRequest(resFactory.CreateErrorResponse("InvalidField", "", null));
            }

            try
            {
                bool chkSuccess = await appService.UpdateAppAsync(app);
                if (chkSuccess)
                {
                    return Ok(resFactory.CreateSuccessResponse("Success", null));
                }

                return Ok(resFactory.CreateErrorResponse("Update failed", "", null));
            }
            catch (DataValidationException ex)
            {
                return Ok(resFactory.CreateErrorResponse(ex.ErrorCode, ex.Message, null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, resFactory.CreateInternalErrorResponse(ServiceError.UnexpectedErrorE999, ex.Message, null));
            }
        }

        /// <summary>
        /// This endpoint is used to get all apps.
        /// </summary>
        [HttpGet("search")]
        [Authorize(Policy = "Admin")]
        public async Task<ActionResult> SearchAppAsync()
        {
            try
            {
                ICollection<App> apps = await appService.GetAppsByCriteriaAsync(new AppSearchCriteria());
                return Ok(resFactory.CreateSuccessResponse(apps, null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, resFactory.CreateInternalErrorResponse(ServiceError.UnexpectedErrorE999, ex.Message, null));
            }
        }

        /// <summary>
        /// This endpoint is used to get the app info by app id.
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Policy = "Admin")]
        public async Task<ActionResult<ApiResponse<object, string>>> GetAppInfo(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(resFactory.CreateErrorResponse("InvalidField", "", null));
            }

            try
            {
                App? app = await appService.GetAppByIdAsync(id);
                if (app == null)
                {
                    return NoContent();
                }

                return Ok(resFactory.CreateSuccessResponse(app, null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, resFactory.CreateInternalErrorResponse(ServiceError.UnexpectedErrorE999, ex.Message, null));
            }
        }

        /// <summary>
        /// This endpoint is used to assign a user to an app.
        /// </summary>
        [HttpPost("{id}/assign-user")]
        [Authorize(Policy = "Admin")]
        public async Task<ActionResult<ApiResponse<object, string>>> CreateUserApp(int id, RequestAddUserApp reqUserId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(FormatInvalidFieldResponse(GetFieldErrors()));
            }

            try
            {
                await usersAppService.AddUserAppAsync(id, reqUserId);

                return Ok(resFactory.CreateSuccessResponse("Success", null));
            }
            catch (ArgumentNullException ex)
            {
                return Ok(FormatDataErrorResponse(ex.Message, "ArgumentNullError"));
            }
            catch (Exception ex)
            {
                return Ok(FormatInternalErrorReponse(ex.Message, null));
            }
        }

        /// <summary>
        /// This endpoint is used to get the list of roles for a specific app.
        /// </summary>
        [HttpGet("{id}/get-roles")]
        [Authorize(Policy = "Admin")]
        public async Task<ActionResult<ApiResponse<object, string>>> GetRoles(int id, [FromQuery] EnumEnvironmentType env)
        {
            try
            {
                ICollection<Role> roles = await targetAppService.GetAppRolesByAppIdAndEnvAsync(id, env);
                return Ok(resFactory.CreateSuccessResponse(roles, null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, resFactory.CreateInternalErrorResponse(ServiceError.UnexpectedErrorE999, ex.Message, null));
            }
        }

        /// <summary>
        /// This endpoint is used to get the JWT token for the target app.
        /// </summary>
        /// TODO: Discuss with the team about the endpoint name.
        [HttpGet("{id}/redirect-target-app")]
        [Authorize(Policy = "Admin")]
        public async Task<ActionResult<ApiResponse<object, string>>> GetJWTTokenForTargetApp(int id, [FromQuery] EnumEnvironmentType env)
        {
            try
            {
                var userClaims = GetUserClaims();
                if (!userClaims.IsValid)
                {
                    return Unauthorized(FormatInternalErrorReponse("User claims not found", null));
                }

                UserApps userApps = await userService.GetUserAppsAsync(userClaims.UserId, env);

                var user = userApps?.User;
                if (user == null)
                {
                    return BadRequest(new ApiResponse<object, string>
                    {
                        Data = null,
                    });
                }

                var (appEnv, jwtTokenWithUser) = await usersAppService.GetJWTTokenTargetAppWithData(id, env, user, userApps?.App ?? []);

                var app = userApps?.App.FirstOrDefault(a => a.AppId == id);
                if (app == null)
                {
                    return BadRequest(new ApiResponse<object, string>
                    {
                        Data = null,
                    });
                }

                var response = new
                {
                    BaseURL = app.BaseUrl,
                    appEnv.EnvironmentType,
                    jwtTokenWithUser.Token,
                    jwtTokenWithUser.ExpiresTime
                };

                return Ok(new ApiResponse<object, string>
                {
                    Data = response,
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, resFactory.CreateInternalErrorResponse(ServiceError.UnexpectedErrorE999, ex.Message, null));
            }
        }
    }
}