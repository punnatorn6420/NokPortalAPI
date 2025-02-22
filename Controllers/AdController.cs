using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NokCore.Api.Controllers;
using NokCore.Api.JWT.Models;
using NokCore.Exceptions;
using NokPortalAPI.Models;
using NokPortalAPI.Services.Old;

namespace NokPortalAPI.Controllers
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
                var linkAD = userService.GenerateAuthorizationUrlSignup();

                return Ok(FormatSuccessResponse(new { link = linkAD }));
            }
            catch (Exception ex)
            {
                return Ok(FormatInternalErrorReponse(ex.Message, null));
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
                if (!ModelState.IsValid)
                {
                    return BadRequest(FormatInvalidFieldResponse(GetFieldErrors()));
                }

                await userService.SignupMicrosoftGraphGetMeAsync(req.Token);

                return Ok(FormatSuccessResponse("Success"));
            }
            catch (DataValidationException ex)
            {
                return Ok(FormatInternalErrorReponse(ex.Message, null));
            }
            catch (Exception ex)
            {
                if (ex.Message == "Response status code does not indicate success: 401 (Unauthorized).")
                {
                    return Ok(FormatInternalErrorReponse("Token is either not in correct format or has expired", null));
                }
                else
                {
                    return Ok(FormatInternalErrorReponse(ex.Message, null));
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
                var linkAD = userService.GenerateAuthorizationUrlSignin();

                return Ok(FormatSuccessResponse(new { link = linkAD }));
            }
            catch (Exception ex)
            {
                return Ok(FormatInternalErrorReponse(ex.Message, null));
            }
        }

        /// <summary>
        /// This endpoint is used to sign in with Microsoft Graph.
        /// </summary>
        [HttpPost("signin")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<object, string>>> SiginAsync(RequestMicrosoftToken req)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(FormatInvalidFieldResponse(GetFieldErrors()));
            }

            try
            {
                JwtResponse jwt_token = await userService.SigninMicrosoftGraphGetMeAsync(req.Token);

                return Ok(FormatSuccessResponse(jwt_token));
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
