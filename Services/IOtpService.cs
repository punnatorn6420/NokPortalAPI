using NokPortalAPI.Dtos.Otp;

namespace NokPortalAPI.Services
{
    /// <summary>
    /// OTP service interface.
    /// </summary>
    public interface IOtpService
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="req"></param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<SendOtpResponseDto> SendAsync(SendOtpRequestDto req);

        /// <summary>
        ///
        /// </summary>
        /// <param name="email"></param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<SendOtpResponseDto> GenerateDecoyAsync(string email);

        /// <summary>
        ///
        /// </summary>
        /// <param name="req"></param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<string> VerifyAsync(VerifyOtpRequestDto req);
    }
}