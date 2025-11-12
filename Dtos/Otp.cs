using NokPortalAPI.Enums;

namespace NokPortalAPI.Dtos.Otp;

/// <summary>
/// Data transfer objects for OTP operations.
/// </summary>
public sealed record SendOtpRequestDto(string Email, OtpChannel Channel = OtpChannel.Email);