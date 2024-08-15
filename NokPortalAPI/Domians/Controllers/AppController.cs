using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using NokCore.Api.Controllers;
using NokCore.Api.JWT.Services;
using NokCore.Exceptions;
using NokCore.Identity.Models;
using NokPortalAPI.Domains.Models;
using NokPortalAPI.Domains.Services;

namespace NokPortalAPI.Domains.Controllers
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
            this.appEnvService = appsEnvService;
            this.usersAppService = usersAppService;
            this.userService = userService;
        }

        /// <summary>
        /// This endpoint is used to create a new app.
        /// </summary>
        [HttpPost("")]
        public async Task<ActionResult> CreateApp(App app)
        {
            // Check if the model state is valid
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.FormatInvalidFieldResponse(this.GetFieldErrors()));
            }

            try
            {
                bool chkSuccess = await this.appService.CreateAppAsync(app);
                if (chkSuccess)
                {
                    return this.Ok(this.FormatSuccessResponse("Success"));
                }

                throw new Exception("Create failed.");
            }
            catch (DataValidationException ex)
            {
                return this.Ok(this.FormatInternalErrorReponse(ex.Message, null));
            }
            catch (Exception ex)
            {
                return this.Ok(this.FormatInternalErrorReponse(ex.Message, null));
            }
        }

        /// <summary>
        /// This endpoint is used to update an existing app.
        /// </summary>
        [HttpPut("")]
        public async Task<ActionResult> UpdateApp(App app)
        {
            // Check if the model state is valid
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.FormatInvalidFieldResponse(this.GetFieldErrors()));
            }

            try
            {
                bool chkSuccess = await this.appService.UpdateAppAsync(app);
                if (chkSuccess)
                {
                    return this.Ok(this.FormatSuccessResponse("Success"));
                }

                throw new Exception("Create failed.");
            }
            catch (DataValidationException ex)
            {
                return this.Ok(this.FormatInternalErrorReponse(ex.Message, null));
            }
            catch (Exception ex)
            {
                return this.Ok(this.FormatInternalErrorReponse(ex.Message, null));
            }
        }

        /// <summary>
        /// This endpoint is used to get all apps.
        /// </summary>
        /// TODO: Discuss with the team about the endpoint name.
        [HttpGet("all")]
        public async Task<ActionResult> GetAllApp()
        {
            try
            {
                IEnumerable<App> app = await appService.GetAllAppsAsync();
                return this.Ok(this.FormatSuccessResponse(app));
            }
            catch (Exception ex)
            {
                return this.Ok(this.FormatInternalErrorReponse(ex.Message, null));
            }
        }

        /// <summary>
        /// This endpoint is used to get all app environments.
        /// </summary>
        /// TODO: Discuss with the team about the endpoint name.
        [HttpGet("env-all")]
        public async Task<ActionResult> GetAllAppEnv()
        {
            try
            {
                IEnumerable<AppEnv> apps = await appEnvService.GetAllAppEnvAsync();
                return this.Ok(this.FormatSuccessResponse(apps));
            }
            catch (Exception ex)
            {
                return this.Ok(this.FormatInternalErrorReponse(ex.Message, null));
            }
        }

        /// <summary>
        /// This endpoint is used to create a new app environment.
        /// </summary>
        [HttpPost("{id}/env")]
        public async Task<ActionResult<ApiResponse<object, string>>> CreateAppEnvironment(int id, AppEnv appEnv)
        {
            if (!ModelState.IsValid)
            {
                return this.BadRequest(this.FormatInvalidFieldResponse(this.GetFieldErrors()));
            }

            try
            {
                appEnv.AppId = id;
                await this.appEnvService.CreateAppEnvAsync(appEnv);
                return this.Ok(this.FormatSuccessResponse("Success"));
            }
            catch (SqlException sqlEx)
            {
                if (sqlEx.Number == 2627)
                {
                    return this.Ok(FormatDataErrorResponse("This app environment already exists in the system", null));
                }
                else
                {
                    return this.Ok(this.FormatInternalErrorReponse(sqlEx.Message, null));
                }
            }
            catch (Exception ex)
            {
                return this.Ok(this.FormatInternalErrorReponse(ex.Message, null));
            }
        }

        /// <summary>
        /// This endpoint is used to update an existing app environment.
        /// </summary>
        [HttpPut("{id}/env")]
        public async Task<ActionResult<ApiResponse<object, string>>> UpdateAppEnvironment(int id, AppEnv appEnv)
        {
            if (!ModelState.IsValid)
            {
                return this.BadRequest(this.FormatInvalidFieldResponse(this.GetFieldErrors()));
            }

            try
            {
                appEnv.AppId = id;
                await this.appEnvService.UpdateAppEnvAsync(appEnv);
                return this.Ok(this.FormatSuccessResponse("Success"));
            }
            catch (DataNotFoundException ex)
            {
                return this.Ok(this.FormatInternalErrorReponse(ex.Message, null));
            }
            catch (Exception ex)
            {
                return this.Ok(this.FormatInternalErrorReponse(ex.Message, null));
            }
        }

        /// <summary>
        /// This endpoint is used to get the app info by app id.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<object, string>>> GetAppInfo(int id)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.FormatInvalidFieldResponse(this.GetFieldErrors()));
            }

            try
            {
                App app = await this.appService.GetAppByIdAsync(id);
                return this.Ok(this.FormatSuccessResponse(app));
            }
            catch (Exception ex)
            {
                return Ok(this.FormatInternalErrorReponse(ex.Message, null));
            }
        }

        /// <summary>
        /// This endpoint is used to assign a user to an app.
        /// </summary>
        [HttpPost("{id}/assign-user")] // Link app, user, (role)
        public async Task<ActionResult<ApiResponse<object, string>>> CreateUserApp(int id, RequestAddUserApp reqUserId)
        {
            if (!ModelState.IsValid)
            {
                return this.BadRequest(this.FormatInvalidFieldResponse(this.GetFieldErrors()));
            }

            try
            {
                await this.usersAppService.AddUserAppAsync(id, reqUserId);

                return this.Ok(this.FormatSuccessResponse("Success"));
            }
            catch (ArgumentNullException ex)
            {
                return this.Ok(this.FormatDataErrorResponse(ex.Message, "ArgumentNullError"));
            }
            catch (SqlException sqlEx)
            {
                if (sqlEx.Number == 2627)
                {
                    return this.Ok(this.FormatInternalErrorReponse("This user already exists in the application", null));
                }
                else
                {
                    return this.Ok(this.FormatInternalErrorReponse(sqlEx.Message, null));
                }
            }
            catch (Exception ex)
            {
                return Ok(this.FormatInternalErrorReponse(ex.Message, null));
            }
        }

        /// <summary>
        /// This endpoint is used to get the list of roles for a specific app.
        /// </summary>
        /// TODO: Discuss with the team about the endpoint name.
        [HttpGet("{id}/get-roles")]
        public async Task<ActionResult<ApiResponse<object, string>>> GetRoles(int id, [FromQuery] EnumEnvironmentType env)
        {
            try
            {
                IEnumerable<Role> app = await this.appService.GetAppTargetAllRole(id, env);
                return this.Ok(this.FormatSuccessResponse(app));
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
        public async Task<ActionResult<ApiResponse<object, string>>> GetJWTTokenForTargetApp(int id, [FromQuery] EnumEnvironmentType env)
        {
            try
            {
                var userClaims = this.GetUserClaims();
                if (!userClaims.IsValid)
                {
                    return this.Unauthorized(this.FormatInternalErrorReponse("User claims not found", null));
                }

                UserApps userApps = await this.userService.GetUserAppsAsync(userClaims.UserId, env);

                var user = userApps?.User;
                if (user == null)
                {
                    return this.BadRequest(new ApiResponse<object, string>
                    {
                        Data = null,
                    });
                }

                var (appEnv, jwtTokenWithUser) = await this.usersAppService.GetJWTTokenTargetAppWithData(id, env, user, userApps?.App ?? []);

                var app = userApps?.App.FirstOrDefault(a => a.AppId == id);
                if (app == null)
                {
                    return this.BadRequest(new ApiResponse<object, string>
                    {
                        Data = null,
                    });
                }

                var response = new
                {
                    BaseURL = app.BaseUrl,
                    appEnv.Environment,
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
                return Ok(this.FormatInternalErrorReponse(ex.Message, null));
            }
        }
    }
}