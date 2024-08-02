using System.Data;
using System.Data.SqlClient;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using NokCore.Api.Controllers;
using NokCore.Api.JWT.Services;
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
        private readonly IAppEnvService appsEnvService;
        private readonly IManagePayloadService managePayload;
        private readonly IUserAppsService usersAppService;
        private readonly IUserService userService;


        public AppController(
            IAppService appService,
            IAppEnvService appsEnvService,
            IManagePayloadService managePayload,
            IUserAppsService usersAppService,
            IUserService userService)
        {
            this.appService = appService;
            this.appsEnvService = appsEnvService;
            this.managePayload = managePayload;
            this.usersAppService = usersAppService;
            this.userService = userService;
        }

        [HttpPost("")]
        public async Task<ActionResult> CreateApp(RequestApp reqApp)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.FormatInvalidFieldResponse(this.GetFieldErrors()));
            }

            int userId = this.managePayload.GetUserIdFromJwtDecode(this.HttpContext);

            try
            {
                bool chkSuccess = await this.appService.CreateAppAsync(reqApp);
                if (chkSuccess)
                {
                    return this.Ok(this.FormatSuccessResponse("Success"));
                }
                throw new Exception("Create failed.");
            }
            catch (DuplicateNameException ex)
            {
                return this.Ok(this.FormatInternalErrorReponse(ex.Message, null));
            }
            catch (Exception ex)
            {
                return this.Ok(this.FormatInternalErrorReponse(ex.Message, null));
            }
        }

        [HttpPut("")]
        public async Task<ActionResult> UpdateApp(RequestApp reqApp)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.FormatInvalidFieldResponse(this.GetFieldErrors()));
            }

            int userId = this.managePayload.GetUserIdFromJwtDecode(this.HttpContext);

            try
            {
                bool chkSuccess = await this.appService.UpdateAppAsync(reqApp);
                if (chkSuccess)
                {
                    return this.Ok(this.FormatSuccessResponse("Success"));
                }
                throw new Exception("Create failed.");
            }
            catch (DuplicateNameException ex)
            {
                return this.Ok(this.FormatInternalErrorReponse(ex.Message, null));
            }
            catch (Exception ex)
            {
                return this.Ok(this.FormatInternalErrorReponse(ex.Message, null));
            }
        }

        [HttpGet("all")]
        public async Task<ActionResult> GetAllApp()
        {
            try
            {
                IEnumerable<App> app = await appService.GetAllAppAsync();
                return this.Ok(this.FormatSuccessResponse(app));
            }
            catch (Exception ex)
            {
                return this.StatusCode(500, this.FormatInternalErrorReponse(ex.Message, null));
            }
        }

        [HttpGet("env-all")]
        public async Task<ActionResult> GetAllAppEnv()
        {
            try
            {
                IEnumerable<AppEnv> apps = await appService.GetAllAppEnvAsync();
                var responseApps = apps.Select(app => new ResponseAppEnv
                {
                    AppId = app.AppId,
                    Environment = app.Environment,
                    BaseURL = app.BaseURL,
                    Additional = app.Additional,
                    JwtHourLimit = app.JwtHourLimit
                });
                return this.Ok(this.FormatSuccessResponse(responseApps));
            }
            catch (Exception ex)
            {
                return this.StatusCode(500, this.FormatInternalErrorReponse(ex.Message, null));
            }
        }

        [HttpPost("{id}/env")]
        public async Task<ActionResult<ApiResponse<object, string>>> CreateAppEnvironment(int id, AppEnv reqAppEnv)
        {
            if (!ModelState.IsValid)
            {
                return this.BadRequest(this.FormatInvalidFieldResponse(this.GetFieldErrors()));
            }

            try
            {
                await this.appsEnvService.CreateAppsEnv(reqAppEnv, id);
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
                    return this.StatusCode(500, this.FormatInternalErrorReponse(sqlEx.Message, null));
                }
            }
            catch (Exception ex)
            {
                return this.StatusCode(500, this.FormatInternalErrorReponse(ex.Message, null));
            }
        }

        [HttpPut("{id}/env")]
        public async Task<ActionResult<ApiResponse<object, string>>> UpdateAppEnvironment(int id, AppEnv reqAppEnv)
        {
            if (!ModelState.IsValid)
            {
                return this.BadRequest(this.FormatInvalidFieldResponse(this.GetFieldErrors()));
            }

            try
            {
                await this.appsEnvService.UpdateAppsEnv(reqAppEnv, id);
                return this.Ok(this.FormatSuccessResponse("Success"));
            }
            catch (KeyNotFoundException ex)
            {
                return this.Ok(this.FormatInternalErrorReponse(ex.Message, null));
            }
            catch (Exception ex)
            {
                return this.StatusCode(500, this.FormatInternalErrorReponse(ex.Message, null));
            }
        }

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
                return StatusCode(500, this.FormatInternalErrorReponse(ex.Message, null));
            }
        }

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
                    return this.StatusCode(500, this.FormatInternalErrorReponse(sqlEx.Message, null));
                }
            }
            catch (Exception ex)
            {
                return this.StatusCode(500, this.FormatInternalErrorReponse(ex.Message, null));
            }
        }

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
                return StatusCode(500, FormatInternalErrorReponse(ex.Message, null));
            }
        }

        [HttpGet("{id}/redirect-target-app")]
        public async Task<ActionResult<ApiResponse<object, string>>> GetJWTTokenForTargetApp(int id, [FromQuery] EnumEnvironmentType env)
        {
            try
            {
                int userId = this.managePayload.GetUserIdFromJwtDecode(this.HttpContext);

                UserApps userApps = await this.userService.GetUserAppsAsync(userId, env);

                var user = userApps?.User;
                if (user == null)
                {
                    return this.BadRequest(new ApiResponse<object, string>
                    {
                        Data = null,
                    });
                }

                var (appEnv, jwtTokenWithUser) = await this.usersAppService.GetJWTTokenTargetAppWithData(id, env, user, userApps.App);

                var app = userApps.App.FirstOrDefault(a => a.AppId == id);
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
                return this.StatusCode(500, this.FormatInternalErrorReponse(ex.Message, null));
            }
        }
    }
}