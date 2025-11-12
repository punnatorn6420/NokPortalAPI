using NokPortalAPI.Enums;

namespace NokPortalAPI.Dtos.Otp;

/// <summary>
/// Data transfer object for verifying an OTP request.
/// </summary>
public sealed record VerifyOtpRequestDto(string VerifyToken, string Email, string Code);