using Microsoft.AspNetCore.Mvc;
using NokCore.Api.Controllers;
using NokCore.Api.JwtToken.Services;
using NokCore.Identity.Models;
using NokPortal.Domains.Services;
using NokPortal.Domians.Models;
using NokPortal.Domians.Services;
using NokPortalAPI.Domians.Models;

namespace NokPortal.Domians.Controllers
{
    [ApiController]
    [Route("me")]
    public class MeController : NokController<ControllerBase>
    {
        private readonly IAuthService authService;
        private readonly IManagePayloadService managePayload;
        private readonly IUserAppsService userAppsService;
        private readonly IUserService userService;

        public MeController(IAuthService authService, IManagePayloadService managePayload, IUserAppsService userAppsService, IUserService userService)
        {
            this.authService = authService;
            this.managePayload = managePayload;
            this.userAppsService = userAppsService;
            this.userService = userService;
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
                int userId = this.managePayload.GetUserIdFromJwtDecode(this.HttpContext);

                User appUser = await this.authService.GetUsersByIdAsync(userId);

                return this.Ok(this.FormatSuccessResponse(appUser));
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

        [HttpGet("info")]
        public async Task<ActionResult<ApiResponse<object, string>>> MeApp()
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.FormatInvalidFieldResponse(this.GetFieldErrors()));
            }

            try
            {
                int userId = this.managePayload.GetUserIdFromJwtDecode(this.HttpContext);

                IEnumerable<UserApps> user = await this.userService.GetUserAppsAsync(userId);

                return this.Ok(this.FormatSuccessResponse(user));
            }
            catch (Exception ex)
            {
                if (ex.Message == "Response status code does not indicate success: 401 (Unauthorized).")
                {
                    return this.Unauthorized(this.FormatInternalErrorReponse("Token is either not in correct format or has expired", null));
                }
                else
                {
                    return this.StatusCode(500, this.FormatInternalErrorReponse(ex.Message, null));
                }
            }
        }
    }
}