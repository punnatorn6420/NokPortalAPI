using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NokCore.Api.JWT.Models;
using NokCore.Api.JWT.Services;
using NokCore.Errors;
using NokCore.Exceptions;
using NokPortalAPI.Models;
using NokPortalAPI.Responses;
using NokPortalAPI.Services;

namespace NokPortalAPI.Controllers
{
    [ApiController]
    [Route("ad")]
    public class AdController : ControllerBase
    {
        private readonly ServiceSettings serviceSettings;
        private readonly MsActiveDirectoryService msActiveDirectoryService;
        private readonly IUserService<User> userService;
        private readonly IJwtService jwtService;
        private readonly ApiResponseFactory apiResponseFactory;

        public AdController(
            IOptions<ServiceSettings> options,
            MsActiveDirectoryService msActiveDirectoryService,
            IUserService<User> userService,
            IJwtService jwtService,
            ApiResponseFactory apiResponseFactory)
        {
            this.serviceSettings = options.Value;
            this.msActiveDirectoryService = msActiveDirectoryService;
            this.userService = userService;
            this.jwtService = jwtService;
            this.apiResponseFactory = apiResponseFactory;
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
                return await Task.FromResult(Ok(apiResponseFactory.CreateSuccessResponse<object>(new { link = authorizationUrl }, null)));
            }
            catch (Exception ex)
            {
                return StatusCode(500, apiResponseFactory.CreateInternalErrorResponse(ServiceError.UnexpectedErrorE999, ex.Message, null));
            }
        }

        /// <summary>
        /// This endpoint is used to sign up with Microsoft Graph.
        /// </summary>
        [HttpPost("signup")]
        [AllowAnonymous]
        public async Task<ActionResult> SignUpAsync(RequestMicrosoftToken req)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(apiResponseFactory.CreateMessageResponse(""));
                }

                // Retrieve the Microsoft user info using the token.
                var msUserInfo = await msActiveDirectoryService.GetMicrosoftUserInfoByTokenAsync(req.Token);

                if (msUserInfo == null)
                {
                    return BadRequest(apiResponseFactory.CreateMessageResponse("Failed to get user info"));
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

                return Ok(apiResponseFactory.CreateSuccessResponse("Success", null));
            }
            catch (DataValidationException ex)
            {
                return Ok(apiResponseFactory.CreateErrorResponse(ex.ErrorCode, ex.Message, null));
            }
            catch (Exception ex)
            {
                if (ex.Message == "Response status code does not indicate success: 401 (Unauthorized).")
                {
                    return BadRequest(apiResponseFactory.CreateMessageResponse("Token is either not in correct format or has expired"));
                }
                else
                {
                    return StatusCode(500, apiResponseFactory.CreateInternalErrorResponse(ServiceError.UnexpectedErrorE999, ex.Message, null));
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
                return await Task.FromResult(Ok(apiResponseFactory.CreateSuccessResponse<object>(new { link = authorizationUrl }, null)));
            }
            catch (Exception ex)
            {
                return StatusCode(500, apiResponseFactory.CreateInternalErrorResponse(ServiceError.UnexpectedErrorE999, ex.Message, null));
            }
        }

        /// <summary>
        /// This endpoint is used to sign in with Microsoft Graph.
        /// </summary>
        [HttpPost("signin")]
        [AllowAnonymous]
        public async Task<ActionResult> SigInAsync(RequestMicrosoftToken req)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(apiResponseFactory.CreateMessageResponse(""));
            }

            try
            {
                var msUserInfo = await msActiveDirectoryService.GetMicrosoftUserInfoByTokenAsync(req.Token);
                if (msUserInfo == null)
                {
                    return BadRequest(apiResponseFactory.CreateMessageResponse("Failed to get user info"));
                }

                var user = await userService.GetUserByEmailAsync(msUserInfo.UserPrincipalName);
                if (user == null)
                {
                    return Ok(apiResponseFactory.CreateMessageResponse("User not found"));
                }

                var jwtData = new UserClaims
                {
                    UserId = user.Id,
                };

                var token = await jwtService.GenerateToken(jwtData);
                return Ok(apiResponseFactory.CreateSuccessResponse(token, null));
            }
            catch (Exception ex)
            {
                if (ex.Message == "Response status code does not indicate success: 401 (Unauthorized).")
                {
                    return Unauthorized(apiResponseFactory.CreateMessageResponse("Token is either not in correct format or has expired"));
                }
                else
                {
                    return StatusCode(500, apiResponseFactory.CreateInternalErrorResponse(ServiceError.UnexpectedErrorE999, ex.Message, null));
                }
            }
        }
    }
}
