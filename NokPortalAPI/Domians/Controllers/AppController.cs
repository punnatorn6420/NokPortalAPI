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
                bool chkSuccess = await _appService.CreateAppAsync(reqApp);
                if (chkSuccess)
                {
                    return Ok(FormatSuccessResponse("Success"));
                }
                throw new Exception("Create failed.");
            }
            catch (DuplicateNameException ex)
            {
                return Ok(FormatInternalErrorReponse(ex.Message, null));
            }
            catch (Exception ex)
            {
                return Ok(FormatInternalErrorReponse(ex.Message, null));
            }
        }

        [HttpGet("all")]
        public async Task<ActionResult> GetAllApp()
        {
            try
            {
                IEnumerable<ModelApp> app = await _appService.GetAllAppAsync();
                return Ok(FormatSuccessResponse(app));
            }
            catch (Exception ex)
            {
                return StatusCode(500, FormatInternalErrorReponse(ex.Message, null));
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<object, string>>> CreateAppEnvironment(int id, AppEnv reqAppEnv)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(FormatInvalidFieldResponse(GetFieldErrors()));
            }

            try
            {
                await _appsEnvService.CreateAppsEnv(reqAppEnv, id);
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
                    return StatusCode(500, FormatInternalErrorReponse(sqlEx.Message, null));
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, FormatInternalErrorReponse(ex.Message, null));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<object, string>>> GetAppInfo(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(FormatInvalidFieldResponse(GetFieldErrors()));
            }

            try
            {
                ModelApp app = await _appService.GetAppByIdAsync(id);
                return Ok(FormatSuccessResponse(app));
            }
            catch (Exception ex)
            {
                return StatusCode(500, FormatInternalErrorReponse(ex.Message, null));
            }
        }

        [HttpPost("{id}/assign-user")] // Link app, user, (role)
        public async Task<ActionResult<ApiResponse<object, string>>> CreateUserApp(int id, RequestAddUserApp reqUserId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(FormatInvalidFieldResponse(GetFieldErrors()));
            }

            int userId = _managePayload.GetUserIdFromJwtDecode(HttpContext);

            // ResponseJwt jwtTokenTargetApp = await _appService.AssignUserTargetAppAsync(id, userId); // mock up
            try
            {
                await _usersAppService.AddUserAppAsync(new ModelUserApp
                {
                    AppId = id,
                    RoleId = EnumUserRole.User,
                    UserId = reqUserId.UserId,
                });
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
        public async Task<ActionResult<ApiResponse<object, string>>> GetRoles(int id)
        {
            try
            {
                IEnumerable<Role> app = await _appService.GetAppTargetAllRole(id);
                return Ok(FormatSuccessResponse(app));
            }
            catch (Exception ex)
            {
                return StatusCode(500, FormatInternalErrorReponse(ex.Message, null));
            }
        }

        [HttpGet("{id}/retireve_token")]
        public async Task<ActionResult<ApiResponse<object, string>>> RetireveToken(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(FormatInvalidFieldResponse(GetFieldErrors()));
            }

            int userId = _managePayload.GetUserIdFromJwtDecode(HttpContext);

            try
            {
                IEnumerable<AppEnv> appEnv = await _appsEnvService.GetById(id, userId);
                return Ok(FormatSuccessResponse(appEnv));
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
    }
}