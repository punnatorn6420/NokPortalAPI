using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using NokCore.Api.Controllers;
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
        private readonly IAppService _appService;
        private readonly IAppEnvService _appsEnvService;
        private readonly IManagePayloadService _managePayload;
        private readonly IUserAppsService _usersAppService;

        public AppController(
            IAppService appService,
            IAppEnvService appsEnvService,
            IManagePayloadService managePayload,
            IUserAppsService usersAppService)
        {
            _appService = appService;
            _appsEnvService = appsEnvService;
            _managePayload = managePayload;
            _usersAppService = usersAppService;
        }

        [HttpPost("")]
        public async Task<ActionResult> CreateApp(RequestCreateApp reqApp)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(FormatInvalidFieldResponse(GetFieldErrors()));
            }

            int userId = _managePayload.GetUserIdFromJwtDecode(HttpContext);


            try
            {
                await _appService.CreateAppAsync(reqApp);
                return Ok(FormatSuccessResponse(null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, FormatInternalErrorReponse(ex.Message, null));
            }
        }

        [HttpGet("get-all")]
        public async Task<ActionResult> GetAllApp()
        {
            try
            {
                IEnumerable<App> app = await _appService.GetAllAppAsync();
                return Ok(FormatSuccessResponse(app));
            }
            catch (Exception ex)
            {
                return StatusCode(500, FormatInternalErrorReponse(ex.Message, null));
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<object, string>>> CreateAppEnvironment(
            int id,
            RequestCreateAppEnv reqAppEnv)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(FormatInvalidFieldResponse(GetFieldErrors()));
            }

            try
            {
                await _appsEnvService.CreateAppsEnv(reqAppEnv, id);
                return Ok(FormatSuccessResponse(null));
            }
            catch (SqlException sqlEx)
            {
                if (sqlEx.Number == 2627)
                {
                    return Ok(FormatDataErrorResponse("This app environment already exists in the system", null));
                }
                else
                {
                    return StatusCode(500, FormatInternalErrorReponse(sqlEx.Message, null));
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, FormatInternalErrorReponse(ex.Message, null));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<object, string>>> GetAppEnv(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(FormatInvalidFieldResponse(GetFieldErrors()));
            }

            try
            {
                int userId = _managePayload.GetUserIdFromJwtDecode(HttpContext);

                RequestCreateAppEnv appEnv = await _appsEnvService.GetById(id, userId);

                ResponseAppsEnv appEnvNoneSecretKey = new ResponseAppsEnv
                {
                    AppId = id,
                    Environment = appEnv.Environment,
                    BaseURL = appEnv.BaseURL,
                    Additional = appEnv.Additional,
                };
                return Ok(FormatSuccessResponse(appEnvNoneSecretKey));
            }
            catch (SqlException sqlEx)
            {
                if (sqlEx.Number == 2627)
                {
                    return Ok(FormatInternalErrorReponse("This app environment already exists in the system", null));
                }
                else
                {
                    return StatusCode(500, FormatInternalErrorReponse(sqlEx.Message, null));
                }
            }
            catch (ArgumentNullException ex)
            {
                return Ok(FormatDataErrorResponse(ex.Message, "ArgumentNullError"));
            }
            catch (ArgumentException ex)
            {
                return Ok(FormatDataErrorResponse(ex.Message, "AuthenticationError"));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Ok(FormatDataErrorResponse(ex.Message, "AuthenticationError"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, FormatInternalErrorReponse(ex.Message, null));
            }
        }

        [HttpPut("{id}/add-user")] // Link app, user, (role)
        public async Task<ActionResult<ApiResponse<object, string>>> CreateUserApp(int id, RequestAddUserApp reqUserId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(FormatInvalidFieldResponse(GetFieldErrors()));
            }

            string jwtTokenTargetApp = await _appService.GenerateJwtTargetApp(1);
            int userId = _managePayload.GetUserIdFromJwtDecode(HttpContext);
            UserApp chkRoleUser = await _usersAppService.GetUserAppAsync(id, userId);

            if (chkRoleUser.RoleId != UserRole.Root && chkRoleUser.RoleId != UserRole.Admin) // (int)UserRole.User
            {
                return Unauthorized(FormatInternalErrorReponse("User is not authorized to add a user to this app.", null));
            }

            try
            {
                await _usersAppService.AddUserAppAsync(new UserApp
                {
                    AppId = id,
                    RoleId = UserRole.User,
                    UserId = reqUserId.UserId,
                });
                return Ok(FormatSuccessResponse(null));
            }
            catch (ArgumentNullException ex)
            {
                return Ok(FormatDataErrorResponse(ex.Message, "ArgumentNullError"));
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
                return StatusCode(500, FormatInternalErrorReponse(ex.Message, null));
            }
        }

        [HttpGet("{id}/get-roles")]
        public async Task<ActionResult<ApiResponse<object, string>>> GetRoles(int appId)
        {
            try
            {
                IEnumerable<App> app = await _appService.GetAllAppAsync();
                return Ok(FormatSuccessResponse(app));
            }
            catch (Exception ex)
            {
                return StatusCode(500, FormatInternalErrorReponse(ex.Message, null));
            }
        }
    }
}