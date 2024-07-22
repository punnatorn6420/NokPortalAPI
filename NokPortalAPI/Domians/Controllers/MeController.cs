using Microsoft.AspNetCore.Mvc;
using NokCore.Api.Controllers;
using NokCore.Api.JWT.Services;
using NokPortalAPI.Domains.Models;
using NokPortalAPI.Domains.Services;

namespace NokPortalAPI.Domains.Controllers
{
    [ApiController]
    [Route("me")]
    public class MeController : NokController<ControllerBase>
    {
        private readonly IManagePayloadService managePayload;
        private readonly IUserAppsService userAppsService;
        private readonly IUserService userService;

        public MeController(IManagePayloadService managePayload, IUserAppsService userAppsService, IUserService userService)
        {
            this.managePayload = managePayload;
            this.userAppsService = userAppsService;
            this.userService = userService;
        }

        [HttpGet("")]
        public async Task<ActionResult<ApiResponse<object, string>>> GetMeInfo([FromQuery] EnumEnvironmentType env)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.FormatInvalidFieldResponse(this.GetFieldErrors()));
            }

            try
            {


                int userId = this.managePayload.GetUserIdFromJwtDecode(this.HttpContext);

                IEnumerable<UserApps> user = await this.userService.GetUserAppsAsync(userId, env);

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