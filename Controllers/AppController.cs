using System.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NokCore.Api.Controllers;
using NokCore.Exceptions;
using NokCore.Identity.Models;
using NokPortalAPI.Models;
using NokPortalAPI.Services.Old;

namespace NokPortalAPI.Controllers
{
    [ApiController]
    [Route("app")]
    public class AppController : NokController<ControllerBase>
    {
        private readonly IAppService appService;
        private readonly IAppEnvService appEnvService;
        private readonly IUserAppsService usersAppService;
        private readonly IUserService userService;

        public AppController(
            IAppService appService,
            IAppEnvService appsEnvService,
            IUserAppsService usersAppService,
            IUserService userService)
        {
            this.appService = appService;
            appEnvService = appsEnvService;
            this.usersAppService = usersAppService;
            this.userService = userService;
        }

        /// <summary>
        /// This endpoint is used to create a new app.
        /// </summary>
        [HttpPost("")]
        [Authorize(Policy = "Admin")]
        public async Task<ActionResult> CreateApp(App app)
        {
            // Check if the model state is valid
            if (!ModelState.IsValid)
            {
                return BadRequest(FormatInvalidFieldResponse(GetFieldErrors()));
            }

            try
            {
                bool chkSuccess = await appService.CreateAppAsync(app);
                if (chkSuccess)
                {
                    return Ok(FormatSuccessResponse("Success"));
                }

                throw new Exception("Create failed.");
            }
            catch (DataValidationException ex)
            {
                return Ok(FormatInternalErrorReponse(ex.Message, null));
            }
            catch (Exception ex)
            {
                return Ok(FormatInternalErrorReponse(ex.Message, null));
            }
        }

        /// <summary>
        /// This endpoint is used to update an existing app.
        /// </summary>
        [HttpPut("")]
        [Authorize(Policy = "Admin")]
        public async Task<ActionResult> UpdateApp(App app)
        {
            // Check if the model state is valid
            if (!ModelState.IsValid)
            {
                return BadRequest(FormatInvalidFieldResponse(GetFieldErrors()));
            }

            try
            {
                bool chkSuccess = await appService.UpdateAppAsync(app);
                if (chkSuccess)
                {
                    return Ok(FormatSuccessResponse("Success"));
                }

                throw new Exception("Create failed.");
            }
            catch (DataValidationException ex)
            {
                return Ok(FormatInternalErrorReponse(ex.Message, null));
            }
            catch (Exception ex)
            {
                return Ok(FormatInternalErrorReponse(ex.Message, null));
            }
        }

        /// <summary>
        /// This endpoint is used to get all apps.
        /// </summary>
        /// TODO: Discuss with the team about the endpoint name.
        [HttpGet("all")]
        [Authorize(Policy = "Admin")]
        public async Task<ActionResult> GetAllApp()
        {
            try
            {
                IEnumerable<App> app = await appService.GetAllAppsAsync();
                return Ok(FormatSuccessResponse(app));
            }
            catch (Exception ex)
            {
                return Ok(FormatInternalErrorReponse(ex.Message, null));
            }
        }

        /// <summary>
        /// This endpoint is used to get all app environments.
        /// </summary>
        /// TODO: Discuss with the team about the endpoint name.
        [HttpGet("env-all")]
        [Authorize(Policy = "Admin")]
        public async Task<ActionResult> GetAllAppEnv()
        {
            try
            {
                IEnumerable<AppEnvironment> apps = await appEnvService.GetAllAppEnvAsync();
                return Ok(FormatSuccessResponse(apps));
            }
            catch (Exception ex)
            {
                return Ok(FormatInternalErrorReponse(ex.Message, null));
            }
        }

        /// <summary>
        /// This endpoint is used to create a new app environment.
        /// </summary>
        [HttpPost("{id}/env")]
        [Authorize(Policy = "Admin")]
        public async Task<ActionResult<ApiResponse<object, string>>> CreateAppEnvironment(int id, AppEnvironment appEnv)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(FormatInvalidFieldResponse(GetFieldErrors()));
            }

            try
            {
                appEnv.AppId = id;
                await appEnvService.CreateAppEnvAsync(appEnv);
                return Ok(FormatSuccessResponse("Success"));
            }
            catch (SqlException sqlEx)
            {
                if (sqlEx.Number == 2627)
                {
                    return Ok(FormatDataErrorResponse("This app environment already exists in the system", null));
                }
                else
                {
                    return Ok(FormatInternalErrorReponse(sqlEx.Message, null));
                }
            }
            catch (Exception ex)
            {
                return Ok(FormatInternalErrorReponse(ex.Message, null));
            }
        }

        /// <summary>
        /// This endpoint is used to update an existing app environment.
        /// </summary>
        [HttpPut("{id}/env")]
        [Authorize(Policy = "Admin")]
        public async Task<ActionResult<ApiResponse<object, string>>> UpdateAppEnvironment(int id, AppEnvironment appEnv)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(FormatInvalidFieldResponse(GetFieldErrors()));
            }

            try
            {
                appEnv.AppId = id;
                await appEnvService.UpdateAppEnvAsync(appEnv);
                return Ok(FormatSuccessResponse("Success"));
            }
            catch (DataNotFoundException ex)
            {
                return Ok(FormatInternalErrorReponse(ex.Message, null));
            }
            catch (Exception ex)
            {
                return Ok(FormatInternalErrorReponse(ex.Message, null));
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
                return BadRequest(FormatInvalidFieldResponse(GetFieldErrors()));
            }

            try
            {
                App app = await appService.GetAppByIdAsync(id);
                return Ok(FormatSuccessResponse(app));
            }
            catch (Exception ex)
            {
                return Ok(FormatInternalErrorReponse(ex.Message, null));
            }
        }

        /// <summary>
        /// This endpoint is used to assign a user to an app.
        /// </summary>
        [HttpPost("{id}/assign-user")] // Link app, user, (role)
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

                return Ok(FormatSuccessResponse("Success"));
            }
            catch (ArgumentNullException ex)
            {
                return Ok(FormatDataErrorResponse(ex.Message, "ArgumentNullError"));
            }
            catch (SqlException sqlEx)
            {
                if (sqlEx.Number == 2627)
                {
                    return Ok(FormatInternalErrorReponse("This user already exists in the application", null));
                }
                else
                {
                    return Ok(FormatInternalErrorReponse(sqlEx.Message, null));
                }
            }
            catch (Exception ex)
            {
                return Ok(FormatInternalErrorReponse(ex.Message, null));
            }
        }

        /// <summary>
        /// This endpoint is used to get the list of roles for a specific app.
        /// </summary>
        /// TODO: Discuss with the team about the endpoint name.
        [HttpGet("{id}/get-roles")]
        [Authorize(Policy = "Admin")]
        public async Task<ActionResult<ApiResponse<object, string>>> GetRoles(int id, [FromQuery] EnumEnvironmentType env)
        {
            try
            {
                IEnumerable<Role> app = await appService.GetAppTargetAllRole(id, env);
                return Ok(FormatSuccessResponse(app));
            }
            catch (Exception ex)
            {
                return Ok(FormatInternalErrorReponse(ex.Message, null));
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
                return Ok(FormatInternalErrorReponse(ex.Message, null));
            }
        }
    }
}