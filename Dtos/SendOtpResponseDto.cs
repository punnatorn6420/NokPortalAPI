using NokPortalAPI.Enums;

namespace NokPortalAPI.Dtos.Otp;

/// <summary>
/// Data transfer object for sending an OTP request.
/// </summary>
public sealed record SendOtpResponseDto(string VerifyToken, DateTime ExpiresAt);