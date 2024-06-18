using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NokCore.Api.Controllers;
using NokPortal.Domians.Models;
using NokPortal.Domians.Services;

namespace NokPortal.Domians.Controllers
{
    [ApiController]
    [Route("app-env")]
    public class AppsEnvController : NokController<ControllerBase>
    {
        private readonly IAppsEnvService _appsEnvService;
        private readonly IJsonHelperService _jsonHelper;

        public AppsEnvController(IAppsEnvService appsEnvService, IJsonHelperService jsonHelper)
        {
            _appsEnvService = appsEnvService;
            _jsonHelper = jsonHelper;
        }

        [HttpPost("create")]
        public async Task<ActionResult<ApiResponse<object, string>>> CreateAppEnv(AppsEnvModel reqAppEnv)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.FormatInvalidFieldResponse(this.GetFieldErrors()));
            }

            try
            {
                await _appsEnvService.CreateAppsEnv(reqAppEnv);
                return this.Ok(this.FormatSuccessResponse(null));
            }
            catch (SqlException sqlEx)
            {
                if (sqlEx.Number == 2627)
                {
                    return this.Ok(this.FormatDataErrorResponse("This app environment already exists in the system", null));
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

        [HttpGet("get")]
        public async Task<ActionResult<ApiResponse<object, string>>> GetAppEnv([FromQuery] RequestAppInfo req)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.FormatInvalidFieldResponse(this.GetFieldErrors()));
            }

            try
            {
                var userIdString = this.HttpContext.Items[_jsonHelper.GetJsonPropertyName<JWTsetting>(nameof(JWTsetting.UserId))] as string
                   ?? throw new ArgumentNullException("Not found userId");

                if (!int.TryParse(userIdString, out int userId))
                {
                    throw new ArgumentException("Invalid userId");
                }

                AppsEnvModel appEnv = await _appsEnvService.GetById(req, userId);
                return Ok(this.FormatSuccessResponse(appEnv));
            }
            catch (SqlException sqlEx)
            {
                if (sqlEx.Number == 2627)
                {
                    return this.Ok(this.FormatInternalErrorReponse("This app environment already exists in the system", null));
                }
                else
                {
                    return this.StatusCode(500, this.FormatInternalErrorReponse(sqlEx.Message, null));
                }
            }
            catch (ArgumentNullException ex)
            {
                return this.Ok(this.FormatDataErrorResponse(ex.Message, "ArgumentNullError"));
            }
            catch (ArgumentException ex)
            {
                return this.Ok(this.FormatDataErrorResponse(ex.Message, "AuthenticationError"));
            }
            catch (UnauthorizedAccessException ex)
            {
                return this.Ok(this.FormatDataErrorResponse(ex.Message, "AuthenticationError"));
            }
            catch (Exception ex)
            {
                return this.StatusCode(500, this.FormatInternalErrorReponse(ex.Message, null));
            }
        }
    }
}
