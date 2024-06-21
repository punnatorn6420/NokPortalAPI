using Microsoft.AspNetCore.Mvc;
using NokCore.Api.Controllers;
using NokCore.Identity.Models;
using NokPortal.Domains.Services;
using NokPortal.Domians.Models;
using NokPortal.Domians.Services;

namespace NokPortal.Domians.Controllers
{
    [ApiController]
    [Route("me")]
    public class MeController : NokController<ControllerBase>
    {
        private readonly IAuthService _authService;
        private readonly IManagePayloadService _managePayload;

        public MeController(IAuthService authService, IManagePayloadService managePayload)
        {
            _authService = authService;
            _managePayload = managePayload;
        }

        [HttpGet("")]
        public async Task<ActionResult<ApiResponse<object, string>>> GetUserInfo()
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.FormatInvalidFieldResponse(this.GetFieldErrors()));
            }

            try
            {
                int userId = _managePayload.GetUserIdFromJwtDecode(this.HttpContext);

                User appUser = await _authService.GetUsersByIdAsync(userId);

                return Ok(this.FormatSuccessResponse(appUser));
            }
            catch (ArgumentNullException ex)
            {
                return this.Ok(this.FormatDataErrorResponse(ex.Message, "ArgumentNullError"));
            }
            catch (ArgumentException ex)
            {
                return this.Ok(this.FormatDataErrorResponse(ex.Message, "AuthenticationError"));
            }
            catch (Exception ex)
            {
                if (ex.Message == "Not found user")
                {
                    return this.Ok(this.FormatDataErrorResponse(ex.Message, "NotFoundError"));
                }

                return this.StatusCode(500, this.FormatInternalErrorReponse(ex.Message, null));
            }
        }
    }
}