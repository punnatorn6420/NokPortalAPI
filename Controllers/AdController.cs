using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NokAir.Core.Exceptions;
using NokAir.Shared.Api.Responses.Factories;
using NokAir.Shared.Controllers;
using NokAir.Shared.Security.Services.InHouse;
using NokPortalAPI.Dtos;
using NokPortalAPI.Entities;
using NokPortalAPI.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace NokPortalAPI.Controllers
{
    /// <summary>
    /// Controller for Microsoft Active Directory integration.
    /// </summary>
    [ApiController]
    [Route("v1/ad")]
    public class AdController : InHouseControllerBase
    {
        private readonly MsActiveDirectoryService msActiveDirectoryService;
        private readonly IUserService<UserDto> userService;
        private readonly IJwtService jwtService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdController"/> class.
        /// </summary>
        public AdController(
            MsActiveDirectoryService msActiveDirectoryService,
            IUserService<UserDto> userService,
            IJwtService jwtService,
            IResponseFactory resFactory)
            : base(resFactory)
        {
            this.msActiveDirectoryService = msActiveDirectoryService;
            this.userService = userService;
            this.jwtService = jwtService;
        }

        /// <summary>
        /// This endpoint is used to request a link for signing up with Microsoft Graph.
        /// </summary>
        [HttpGet("get-signup-link")]
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
        public async Task<ActionResult> SignUpAsync(MicrosoftTokenDto req)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequestResponseFromInvalidRequest();
                }
                var msUserInfo = await msActiveDirectoryService.GetMicrosoftUserInfoByTokenAsync(req.Token);

                if (msUserInfo == null)
                {
                    return BadRequestResponseFromMessage("Failed to get user info");
                }

                var existingUserByEmail = await userService.GetUserByEmailAsync(msUserInfo.UserPrincipalName);
                if (existingUserByEmail != null)
                {
                    return BadRequestResponseFromMessage($"User with email {msUserInfo.UserPrincipalName} already exists.");
                }

                UserDto user = new UserDto
                {
                    FirstName = msUserInfo.GivenName ?? string.Empty,
                    LastName = msUserInfo.Surname ?? string.Empty,
                    Email = msUserInfo.UserPrincipalName ?? string.Empty,
                    JobTitle = msUserInfo.JobTitle ?? string.Empty,
                    ObjectId = msUserInfo.Id ?? string.Empty,
                    Department = msUserInfo.OfficeLocation ?? string.Empty,
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
        [HttpGet("get-signin-link")]
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
        /// This endpoint is used to get the JWT token to access the target app for Admin.
        /// </summary>
        [HttpGet("shortlive-token")]
        [AllowAnonymous]
        public async Task<ActionResult> GetShortliveJwtTokenInfoAsync([FromQuery] int userId)
        {
            try
            {
                // Get user info
                var user = await userService.GetUserByIdAsync(userId);
                if (user == null)
                {
                    throw new DataValidationException("User not found. Please check the user.");
                }

                var cliams = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Jti, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim("role", string.Join(",", user.Role)),
                };
                var token = jwtService.GenerateJwtTokenInfo(cliams!);
                return OkResponseWithResult(token);
            }
            catch (DataValidationException ex)
            {
                return BadRequestResponseFromMessage(ex.Message);
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
        public async Task<ActionResult> SigInAsync(MicrosoftTokenDto req)
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

                var cliams = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Jti, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.UniqueName, string.Format("{0} {1}", user.FirstName, user.LastName)),
                };
                var token = jwtService.GenerateJwtTokenInfo(cliams!);
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