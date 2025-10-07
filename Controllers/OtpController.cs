using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NokAir.Shared.Api.Responses.Factories;
using NokAir.Shared.Controllers;
using NokPortalAPI.Dtos;
using NokPortalAPI.Dtos.Otp;
using NokPortalAPI.Enums;
using NokPortalAPI.Services;

namespace NokPortalAPI.Controllers;

[ApiController]
[Route("v1/otp")]
public class OtpController : InHouseControllerBase
{
    private readonly IOtpService otpService;

    public OtpController(IOtpService otpService, IResponseFactory resFactory) : base(resFactory)
    {
        this.otpService = otpService;

    }

    [HttpPost("send")]
    [AllowAnonymous]
    public async Task<IActionResult> SendAsync(
    [FromBody] SendOtpRequestDto dto,
    [FromServices] IUserService<UserDto> userService)
    {
        if (!ModelState.IsValid) return BadRequestResponseFromInvalidRequest();

        var email = dto.Email.Trim().ToLowerInvariant();
        if (dto.Channel != OtpChannel.Email)
            return BadRequestResponseFromErrorCode("unsupported_channel");

        var user = await userService.GetUserByEmailAsync(email);

        var res = user is not null
            ? await otpService.SendAsync(dto with { Email = email, Channel = OtpChannel.Email })
            : await otpService.GenerateDecoyAsync(email);

        return OkResponseWithResult(new
        {
            verifyToken = res.VerifyToken,
            expiresAt = res.ExpiresAt
        });
    }

    [Authorize]
    [HttpPost("verify")]
    public async Task<IActionResult> VerifyAsync([FromBody] VerifyOtpRequestDto dto)
    {
        if (!ModelState.IsValid) return BadRequestResponseFromInvalidRequest();

        try
        {
            var accessToken = await otpService.VerifyAsync(dto);
            return OkResponseWithResult(new VerifyOtpResponseDto(accessToken));
        }
        catch (InvalidOperationException)
        {
            return Unauthorized(new
            {
                message = "The verification code is invalid or has expired. Please try again."
            });
        }
        catch (Exception ex)
        {
            return InternalServerErrorResponseFromException(ex);
        }
    }
}
