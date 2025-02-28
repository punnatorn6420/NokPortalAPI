using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NokCore.Api.Controllers.Internal;
using NokCore.Api.JWT.Models;
using NokCore.Api.JWT.Services;
using NokCore.Api.Responses.Web;
using NokCore.Exceptions;
using NokPortalAPI.Models;
using NokPortalAPI.Services;
using System.Security.Claims;

namespace NokPortalAPI.Controllers
{
    [ApiController]
    [Route("v1/ad")]
    public class AdController : BaseController
    {
        private readonly MsActiveDirectoryService msActiveDirectoryService;
        private readonly IUserService<User> userService;
        private readonly IJwtService jwtService;

        public AdController(
            MsActiveDirectoryService msActiveDirectoryService,
            IUserService<User> userService,
            IJwtService jwtService,
            IApiResponseFactory resFactory) : base(resFactory)
        {
            this.msActiveDirectoryService = msActiveDirectoryService;
            this.userService = userService;
            this.jwtService = jwtService;
        }

        /// <summary>
        /// This endpoint is used to request a link for signing up with Microsoft Graph.
        /// </summary>
        [HttpGet("authorization-link-signup")]
        [AllowAnonymous]
        public async Task<ActionResult> GetAuthorizationLinkSignUpAsync()
        {
            try
            {
                var authorizationUrl = await msActiveDirectoryService.GenerateAuthorizationUrlSignUpAsync();
                return await Task.FromResult(OkResponseWithResult(new { link = authorizationUrl }));
            }
            catch (Exception ex)
            {
                return InternalServerErrorResponseFromException(ex);
            }
        }

        /// <summary>
        /// This endpoint is used to sign up with Microsoft Graph.
        /// </summary>
        [HttpPost("signup")]
        [AllowAnonymous]
        public async Task<ActionResult> SignUpAsync(MicrosoftTokenRequest req)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequestResponseFromInvalidRequest();
                }

                // Retrieve the Microsoft user info using the token.
                var msUserInfo = await msActiveDirectoryService.GetMicrosoftUserInfoByTokenAsync(req.Token);

                if (msUserInfo == null)
                {
                    return BadRequestResponseFromMessage("Failed to get user info");
                }

                User user = new User
                {
                    FirstName = msUserInfo.GivenName,
                    LastName = msUserInfo.Surname,
                    Email = msUserInfo.UserPrincipalName,
                    JobTitle = msUserInfo.JobTitle,
                    ObjectId = msUserInfo.Id,
                    Department = msUserInfo.OfficeLocation,
                    CreatedAt = DateTime.Now,
                    ModifiedAt = DateTime.Now,
                    Active = true,
                };
                await userService.AddUserAsync(user);

                return OkSuccessResponse();
            }
            catch (DataValidationException ex)
            {
                return BadRequestResponseFromErrorCode(ex.ErrorCode);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Response status code does not indicate success: 401 (Unauthorized).")
                {
                    return BadRequestResponseFromMessage("Token is either not in correct format or has expired");
                }
                else
                {
                    return InternalServerErrorResponseFromException(ex);
                }
            }
        }

        /// <summary>
        /// This endpoint is used to request a link for signing in with Microsoft Graph.
        /// </summary>
        [HttpGet("authorization-link-signin")]
        [AllowAnonymous]
        public async Task<ActionResult> GetAuthorizationLinkSignInAsync()
        {
            try
            {
                var authorizationUrl = await msActiveDirectoryService.GenerateAuthorizationUrlSignInAsync();
                return await Task.FromResult(OkResponseWithResult(new { link = authorizationUrl }));
            }
            catch (Exception ex)
            {
                return InternalServerErrorResponseFromException(ex);
            }
        }

        /// <summary>
        /// This endpoint is used to sign in with Microsoft Graph.
        /// </summary>
        [HttpPost("signin")]
        [AllowAnonymous]
        public async Task<ActionResult> SigInAsync(MicrosoftTokenRequest req)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestResponseFromInvalidRequest();
            }

            try
            {
                var msUserInfo = await msActiveDirectoryService.GetMicrosoftUserInfoByTokenAsync(req.Token);
                if (msUserInfo == null)
                {
                    return BadRequestResponseFromMessage("Failed to get user info");
                }

                var user = await userService.GetUserByEmailAsync(msUserInfo.UserPrincipalName);
                if (user == null)
                {
                    return NoContent();
                }

                var jwtData = new JwtUserClaims
                {
                    UserId = user.Id,
                };
                var cliams = new []
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, "Unknown"),
                };
                var token = jwtService.GetJwtTokenInfo(cliams!);
                return OkResponseWithResult(token);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Response status code does not indicate success: 401 (Unauthorized).")
                {
                    return Unauthorized("Token is either not in correct format or has expired");
                }
                else
                {
                    return InternalServerErrorResponseFromException(ex);
                }
            }
        }
    }
}
