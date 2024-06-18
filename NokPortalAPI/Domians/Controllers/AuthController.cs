using Microsoft.AspNetCore.Mvc;
using NokCore.Api.Controllers;
using NokPortal.Domains.Services;
using NokPortal.Domians.Models;
using NokPortal.Domians.Services;

namespace NokPortal.Domians.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : NokController<ControllerBase>
    {
        private readonly IAuthService _authService;
        private readonly IJsonHelperService _jsonHelper;

        public AuthController(IAuthService authService, IJsonHelperService jsonHelper)
        {
            _authService = authService;
            _jsonHelper = jsonHelper;
        }

        [HttpGet("me")]
        public async Task<ActionResult<ApiResponse<object, string>>> GetUserInfo()
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

                Users appUser = await _authService.GetUsersByIdAsync(userId);

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