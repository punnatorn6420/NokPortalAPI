using Microsoft.AspNetCore.Mvc;
using NokCore.Api.Controllers;
using NokPortal.Domains.Models;
using NokPortal.Domians.Services;

namespace NokPortal.Domians.Controllers
{
    [ApiController]
    [Route("user-app")]
    public class UsersAppsController : NokController<ControllerBase>
    {
        private readonly IUsersAppsService _usersAppService;

        public UsersAppsController(IUsersAppsService usersAppService)
        {
            _usersAppService = usersAppService;
        }

        [HttpPost("create")]
        public async Task<ActionResult<ApiResponse<object, string>>> CreateUserApp(UserApp req)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.FormatInvalidFieldResponse(this.GetFieldErrors()));
            }

            try
            {
                await _usersAppService.AddUserAppAsync(req);
                return this.Ok(this.FormatSuccessResponse(null));
            }
            catch (Exception ex)
            {
                return this.StatusCode(500, this.FormatInternalErrorReponse(ex.Message, null));
            }
        }
    }
}
