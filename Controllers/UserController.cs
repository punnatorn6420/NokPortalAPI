using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NokCore.Api.Controllers;
using NokCore.Identity.Models;
using NokPortalAPI.Models;
using NokPortalAPI.Services.Old;

namespace NokPortalAPI.Controllers
{
    [ApiController]
    [Route("user")]
    public class UserController : NokController<ControllerBase>
    {
        private readonly IUserService userService;

        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpGet("all")]
        [Authorize(Policy = "Admin")]
        public async Task<ActionResult<ApiResponse<object, string>>> UserAll()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(FormatInvalidFieldResponse(GetFieldErrors()));
            }

            try
            {
                IEnumerable<User> user = await userService.GetAllUsersAsync();

                return Ok(FormatSuccessResponse(user));
            }
            catch (Exception ex)
            {
                return Ok(FormatInternalErrorReponse(ex.Message, null));
            }
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "Admin")]
        public async Task<ActionResult<ApiResponse<object, string>>> GetByUserId(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(FormatInvalidFieldResponse(GetFieldErrors()));
            }

            try
            {
                UserAppWithEnvRoles user = await userService.GetUserAppsWithEnvRolesAsync(id);
                return Ok(FormatSuccessResponse(user));
            }
            catch (Exception ex)
            {
                if (ex.Message == "Response status code does not indicate success: 401 (Unauthorized).")
                {
                    return Unauthorized(FormatInternalErrorReponse("Token is either not in correct format or has expired", null));
                }
                else
                {
                    return Ok(FormatInternalErrorReponse(ex.Message, null));
                }
            }
        }
    }
}