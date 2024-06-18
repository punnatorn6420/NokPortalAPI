using System.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NokCore.Api.Controllers;
using NokPortal.Domains.Services;
using NokPortal.Domians.Models;

namespace NokPortal.Domians.Controllers
{
    [ApiController]
    [Route("ad")]
    public class UsersController : NokController<ControllerBase>
    {
        private readonly IUsersService _appUserService;

        public UsersController(IUsersService authService)
        {
            _appUserService = authService;
        }

        [HttpGet("authorization-link-signup")]
        [AllowAnonymous]
        public ActionResult RequestLinkSignup()
        {
            try
            {
                var linkAD = _appUserService.GenerateAuthorizationUrlSignup();

                return this.Ok(this.FormatSuccessResponse(new { link = linkAD }));
            }
            catch (Exception ex)
            {
                return this.StatusCode(500, this.FormatInternalErrorReponse(ex.Message, null));
            }
        }

        [HttpPost("signup")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<object, string>>> Signup(RequestToken req)
        {
            try
            {
                if (!this.ModelState.IsValid)
                {
                    return this.BadRequest(this.FormatInvalidFieldResponse(this.GetFieldErrors()));
                }

                await _appUserService.SignupMicrosoftGraphGetMeAsync(req.Token);

                return this.Ok(this.FormatSuccessResponse(null));
            }
            catch (SqlException sqlEx)
            {
                if (sqlEx.Number == 2627)
                {
                return this.Ok(this.FormatInternalErrorReponse("This email already exists in the system", null));
                }
                else
                {
                    return this.StatusCode(500, this.FormatInternalErrorReponse(sqlEx.Message, null));
                }
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
                var linkAD = _appUserService.GenerateAuthorizationUrlSignin();

                return this.Ok(this.FormatSuccessResponse(new { link = linkAD }));
            }
            catch (Exception ex)
            {
                return this.StatusCode(500, this.FormatInternalErrorReponse(ex.Message, null));
            }
        }

        [HttpPost("signin")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<object, string>>> Sigin(RequestToken req)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.FormatInvalidFieldResponse(this.GetFieldErrors()));
            }

            try
            {
                ResponseJwt jwt_token = await _appUserService.SigninMicrosoftGraphGetMeAsync(req.Token);

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
