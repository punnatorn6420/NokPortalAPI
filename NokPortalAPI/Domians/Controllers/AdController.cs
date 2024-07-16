using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NokCore.Api.Controllers;
using NokCore.Api.JwtToken.Models;
using NokCore.Api.JwtToken.Services;
using NokPortalAPI.Domains.Models;
using NokPortalAPI.Domains.Services;

namespace NokPortalAPI.Domains.Controllers
{
    [ApiController]
    [Route("ad")]
    public class AdController : NokController<ControllerBase>
    {
        private readonly IUserService userService;
        private readonly IManagePayloadService managePayload;
        private readonly IUserAppsService userAppsService;

        public AdController(IUserService userService, IManagePayloadService managePayload, IUserAppsService userAppsService)
        {
            this.userService = userService;
            this.managePayload = managePayload;
            this.userAppsService = userAppsService;
        }

        [HttpGet("authorization-link-signup")]
        [AllowAnonymous]
        public ActionResult RequestLinkSignup()
        {
            try
            {
                var linkAD = this.userService.GenerateAuthorizationUrlSignup();

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

                await userService.SignupMicrosoftGraphGetMeAsync(req.Token);

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
                var linkAD = this.userService.GenerateAuthorizationUrlSignin();

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
                ResponseJwt jwt_token = await userService.SigninMicrosoftGraphGetMeAsync(req.Token);

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
    }
}
