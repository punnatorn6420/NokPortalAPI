using NokPortalAPI.Enums;

namespace NokPortalAPI.Dtos.Otp;

public sealed record SendOtpRequestDto(string Email, OtpChannel Channel = OtpChannel.Email);
public sealed record SendOtpResponseDto(string VerifyToken, DateTime ExpiresAt);
public sealed record VerifyOtpRequestDto(string VerifyToken, string Email, string Code);
public sealed record VerifyOtpResponseDto(string AccessToken);
