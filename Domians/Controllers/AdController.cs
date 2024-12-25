using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NokCore.Api.Controllers;
using NokCore.Api.JWT.Models;
using NokCore.Api.JWT.Services;
using NokCore.Exceptions;
using NokPortalAPI.Domains.Models;
using NokPortalAPI.Domains.Services;

namespace NokPortalAPI.Domains.Controllers
{
    [ApiController]
    [Route("ad")]
    public class AdController : NokController<ControllerBase>
    {
        private readonly IUserService userService;
        private readonly IUserAppsService userAppsService;

        public AdController(IUserService userService, IUserAppsService userAppsService)
        {
            this.userService = userService;
            this.userAppsService = userAppsService;
        }

        /// <summary>
        /// This endpoint is used to request a link for signing up with Microsoft Graph.
        /// </summary>
        [HttpGet("authorization-link-signup")]
        [AllowAnonymous]
        public ActionResult GetAuthorizationLinkSignupAsync()
        {
            try
            {
                var linkAD = this.userService.GenerateAuthorizationUrlSignup();

                return this.Ok(this.FormatSuccessResponse(new { link = linkAD }));
            }
            catch (Exception ex)
            {
                return this.Ok(this.FormatInternalErrorReponse(ex.Message, null));
            }
        }

        /// <summary>
        /// This endpoint is used to sign up with Microsoft Graph.
        /// </summary>
        [HttpPost("signup")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<object, string>>> SignupAsync(RequestMicrosoftToken req)
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
            catch (DataValidationException ex)
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
                    return this.Ok(this.FormatInternalErrorReponse(ex.Message, null));
                }
            }
        }

        /// <summary>
        /// This endpoint is used to request a link for signing in with Microsoft Graph.
        /// </summary>
        [HttpGet("authorization-link-signin")]
        [AllowAnonymous]
        public ActionResult<ApiResponse<object, string>> GetAuthorizationLinkSignInAsync()
        {
            try
            {
                var linkAD = this.userService.GenerateAuthorizationUrlSignin();

                return this.Ok(this.FormatSuccessResponse(new { link = linkAD }));
            }
            catch (Exception ex)
            {
                return this.Ok(this.FormatInternalErrorReponse(ex.Message, null));
            }
        }

        /// <summary>
        /// This endpoint is used to sign in with Microsoft Graph.
        /// </summary>
        [HttpPost("signin")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<object, string>>> SiginAsync(RequestMicrosoftToken req)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.FormatInvalidFieldResponse(this.GetFieldErrors()));
            }

            try
            {
                JwtResponse jwt_token = await userService.SigninMicrosoftGraphGetMeAsync(req.Token);

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
                    return this.Ok(this.FormatInternalErrorReponse(ex.Message, null));
                }
            }
        }
    }
}
