using NokPortalAPI.Dtos.Otp;

namespace NokPortalAPI.Services
{
    public interface IOtpService
    {
        Task<SendOtpResponseDto> SendAsync(SendOtpRequestDto req);
        Task<SendOtpResponseDto> GenerateDecoyAsync(string email);
        Task<string> VerifyAsync(VerifyOtpRequestDto req);
    }
}
