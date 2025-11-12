using NokPortalAPI.Enums;

namespace NokPortalAPI.Dtos.Otp;

/// <summary>
/// Data transfer object for the response after verifying an OTP.
/// </summary>
public sealed record VerifyOtpResponseDto(string AccessToken);