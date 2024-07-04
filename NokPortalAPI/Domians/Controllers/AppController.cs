using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using NokCore.Api.Controllers;
using NokCore.Api.JwtToken.Services;
using NokCore.Identity.Models;
using NokPortal.Domains.Models;
using NokPortal.Domians.Models;
using NokPortal.Domians.Services;
using NokPortalAPI.Domians.Models;

namespace NokPortal.Domians.Controllers
{
    [ApiController]
    [Route("app")]
    public class AppController : NokController<ControllerBase>
    {
        private readonly IAppService appService;
        private readonly IAppEnvService appsEnvService;
        private readonly IManagePayloadService managePayload;
        private readonly IUserAppsService usersAppService;

        public AppController(
            IAppService appService,
            IAppEnvService appsEnvService,
            IManagePayloadService managePayload,
            IUserAppsService usersAppService)
        {
            this.appService = appService;
            this.appsEnvService = appsEnvService;
            this.managePayload = managePayload;
            this.usersAppService = usersAppService;
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
                IEnumerable<ModelApp> app = await appService.GetAllAppAsync();
                return this.Ok(this.FormatSuccessResponse(app));
            }
            catch (Exception ex)
            {
                return this.StatusCode(500, this.FormatInternalErrorReponse(ex.Message, null));
            }
        }

        [HttpGet("{id}/jwt")]
        public async Task<ActionResult> GetJWTapplication()
        {
            try
            {
                IEnumerable<ModelApp> app = await appService.GetAllAppAsync();
                return this.Ok(this.FormatSuccessResponse(app));
            }
            catch (Exception ex)
            {
                return this.StatusCode(500, this.FormatInternalErrorReponse(ex.Message, null));
            }
        }

        [HttpPost("{id}/create-env")]
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

        [HttpPut("{id}/update-env")]
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
                ModelApp app = await this.appService.GetAppByIdAsync(id);
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

            int userId = this.managePayload.GetUserIdFromJwtDecode(this.HttpContext);

            // ResponseJwt jwtTokenTargetApp = await _appService.AssignUserTargetAppAsync(id, userId); // mock up
            try
            {
                await this.usersAppService.AddUserAppAsync(new ModelUserApp
                {
                    AppId = id,
                    RoleId = EnumUserRole.User,
                    UserId = reqUserId.UserId,
                });
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

        [HttpGet("{id}/retireve_token")]
        public async Task<ActionResult<ApiResponse<object, string>>> RetireveToken(int id, [FromQuery]EnumEnvironmentType env)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.FormatInvalidFieldResponse(this.GetFieldErrors()));
            }

            int userId = this.managePayload.GetUserIdFromJwtDecode(this.HttpContext);

            try
            {
                AppEnv appEnv = await appsEnvService.GetById(id, userId, env);
                return this.Ok(this.FormatSuccessResponse(appEnv));
            }
            catch (DataException ex)
            {
                return this.Ok(this.FormatSuccessResponse(FormatInternalErrorReponse(ex.Message, null)));
            }
            catch (Exception ex)
            {
                return this.StatusCode(500, this.FormatInternalErrorReponse(ex.Message, null));
            }
        }
    }
}