using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NokCore.Api.Controllers;
using NokCore.Api.JwtToken.Models;
using NokCore.Api.JwtToken.Services;
using NokCore.Identity.Models;
using NokPortal.Domains.Services;
using NokPortal.Domians.Models;
using NokPortal.Domians.Services;
using NokPortalAPI.Domians.Models;

namespace NokPortal.Domians.Controllers
{
    [ApiController]
    [Route("ad")]
    public class UsersController : NokController<ControllerBase>
    {
        private readonly IUserService _userService;
        private readonly IManagePayloadService _managePayload;
        private readonly IUserAppsService _userAppsService;

        public UsersController(IUserService userService, IManagePayloadService managePayloadService, IUserAppsService userAppsService)
        {
            _userService = userService;
            _managePayload = managePayloadService;
            _userAppsService = userAppsService;
        }

        [HttpGet("authorization-link-signup")]
        [AllowAnonymous]
        public ActionResult RequestLinkSignup()
        {
            try
            {
                var linkAD = _userService.GenerateAuthorizationUrlSignup();

                return this.Ok(this.FormatSuccessResponse(new { link = linkAD }));
            }
            catch (Exception ex)
            {
                return this.StatusCode(500, this.FormatInternalErrorReponse(ex.Message, null));
            }
        }

        [HttpPost("signup")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<object, string>>> Signup(RequestMicrosoftToken req)
        {
            try
            {
                if (!this.ModelState.IsValid)
                {
                    return this.BadRequest(this.FormatInvalidFieldResponse(this.GetFieldErrors()));
                }

                await _userService.SignupMicrosoftGraphGetMeAsync(req.Token);

                return this.Ok(this.FormatSuccessResponse("Success"));
            }
            catch (DuplicateNameException ex)
            {
                return this.Ok(this.FormatInternalErrorReponse(ex.Message, null));
            }
            catch (Exception ex)
            {
                if (ex.Message == "Response status code does not indicate success: 401 (Unauthorized).")
                {
                    return this.Ok(this.FormatInternalErrorReponse("Token is either not in correct format or has expired", null));
                }
                else
                {
                    return this.StatusCode(500, this.FormatInternalErrorReponse(ex.Message, null));
                }
            }
        }

        [HttpGet("authorization-link-signin")]
        [AllowAnonymous]
        public ActionResult<ApiResponse<object, string>> RequestLinkSigin()
        {
            try
            {
                var linkAD = _userService.GenerateAuthorizationUrlSignin();

                return this.Ok(this.FormatSuccessResponse(new { link = linkAD }));
            }
            catch (Exception ex)
            {
                return this.StatusCode(500, this.FormatInternalErrorReponse(ex.Message, null));
            }
        }

        [HttpPost("signin")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<object, string>>> Sigin(RequestMicrosoftToken req)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.FormatInvalidFieldResponse(this.GetFieldErrors()));
            }

            try
            {
                ResponseJwt jwt_token = await _userService.SigninMicrosoftGraphGetMeAsync(req.Token);

                return this.Ok(this.FormatSuccessResponse(jwt_token));
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

        [HttpGet("user-all")]
        public async Task<ActionResult<ApiResponse<object, string>>> UserAll()
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.FormatInvalidFieldResponse(this.GetFieldErrors()));
            }

            try
            {
                IEnumerable<User> user = await _userService.GetAllUsersAsync();

                return this.Ok(this.FormatSuccessResponse(user));
            }
            catch (Exception ex)
            {
                return this.StatusCode(500, this.FormatInternalErrorReponse(ex.Message, null));
            }
        }

        [HttpGet("user/{id}")]
        public async Task<ActionResult<ApiResponse<object, string>>> UserApp(int id)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.FormatInvalidFieldResponse(this.GetFieldErrors()));
            }

            try
            {
                IEnumerable<UserApps> user = await _userService.GetUserAppsAsync(id);
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
