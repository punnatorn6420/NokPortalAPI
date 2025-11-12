using Microsoft.Extensions.Options;
using NokAir.Shared.Security.Models.Common;
using NokAir.Shared.Security.Services.InHouse;
using NokPortalAPI.Dtos;
using NokPortalAPI.Dtos.Otp;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace NokPortalAPI.Services
{
    /// <summary>
    /// OTP service.
    /// </summary>
    public class OtpService : IOtpService
    {
        private static readonly TimeSpan Ttl = TimeSpan.FromMinutes(3);
        private readonly IUserService<UserDto> _userService;
        private readonly IJwtService _jwtService;
        private readonly JwtSettingsModel _baseJwt;

        /// <summary>
        /// Initializes a new instance of the <see cref="OtpService"/> class.
        /// </summary>
        /// <param name="userService"></param>
        /// <param name="jwtService"></param>
        /// <param name="jwtOptions"></param>
        public OtpService(
                        IUserService<UserDto> userService,
                        IJwtService jwtService,
                        IOptions<JwtSettingsModel> jwtOptions)
        {
            _userService = userService;
            _jwtService = jwtService;
            _baseJwt = jwtOptions.Value ?? throw new InvalidOperationException("JwtSettings is not bound.");

            if (string.IsNullOrWhiteSpace(_baseJwt.SecretKey) || _baseJwt.SecretKey.Length < 32)
            {
                throw new InvalidOperationException("JwtSettings:SecretKey must be set and >= 32 characters.");
            }

            if (string.IsNullOrWhiteSpace(_baseJwt.Issuer) || string.IsNullOrWhiteSpace(_baseJwt.Audience))
            {
                throw new InvalidOperationException("JwtSettings:Issuer/Audience are required.");
            }
        }

        /// <inheritdoc/>
        public async Task<SendOtpResponseDto> SendAsync(SendOtpRequestDto req)
        {
            var email = req.Email?.Trim();
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new InvalidOperationException("Email is required.");
            }

            var emailLower = email.ToLowerInvariant();

            var salt = Convert.ToHexString(RandomNumberGenerator.GetBytes(16));
            var code = GenerateNumericCode(6);
            var codeHash = Sha256($"{code}:{salt}");
            var expiresAt = DateTime.Now.Add(Ttl);

            var claims = new[]
                {
                new Claim(JwtRegisteredClaimNames.Sub, emailLower),
                new Claim(ClaimTypes.NameIdentifier, emailLower),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                new Claim("typ", "otp"),
                new Claim("otp_salt",  salt),
                new Claim("otp_hash",  codeHash),
                };

            var otpJwtSettings = new JwtSettingsModel
            {
                Issuer = string.IsNullOrWhiteSpace(_baseJwt.Issuer) ? "NokAir" : $"{_baseJwt.Issuer}",
                Audience = string.IsNullOrWhiteSpace(_baseJwt.Audience) ? "NokAir" : $"{_baseJwt.Audience}",
                SecretKey = _baseJwt.SecretKey,
                ExpiryInMinutes = 3
            };

            var jwt = _jwtService.GenerateJwtTokenInfo(claims, otpJwtSettings);
            var verifyToken = jwt.Token!;

            var subject = "Your NokPortal login code";
            var htmlBody = $@"
                    <!DOCTYPE html>
                    <html lang=""en"">
                    <head>
                    <meta charset=""utf-8"">
                    <meta name=""viewport"" content=""width=device-width, initial-scale=1"">
                    <title>NokPortal Login</title>
                    <style>
                        /* preheader (hidden) */
                    </style>
                    </head>
                    <body style=""margin:0; padding:0; background:#f5f7fb;"">
                    <div style=""display:none !important; visibility:hidden; opacity:0; color:transparent; height:0; width:0; overflow:hidden; mso-hide:all;"">
                        Your one-time code for NokPortal.
                    </div>
                    <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" border=""0"" width=""100%"" style=""background:#f5f7fb;"">
                        <tr>
                        <td align=""center"" style=""padding:24px 12px;"">
                            <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" border=""0"" width=""600"" style=""max-width:600px; width:100%; background:#ffffff; border-radius:16px; overflow:hidden; box-shadow:0 6px 18px rgba(0,0,0,.06);"">
                            <tr>
                                <td style=""background:#ffcb0b; padding:18px 24px; text-align:left;"">
                                <div style=""font-family:Segoe UI, Arial, sans-serif; font-weight:800; font-size:18px; color:#111111;"">
                                    NokPortal
                                </div>
                                <div style=""font-family:Segoe UI, Arial, sans-serif; font-size:12px; color:#2c2c2c; opacity:.85;"">
                                    One-Time Password
                                </div>
                                </td>
                            </tr>
                            <tr>
                                <td style=""padding:24px 24px 8px 24px;"">
                                <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"">
                                    <tr>
                                    <td style=""font-family:Segoe UI, Arial, sans-serif; color:#0f172a; font-size:20px; font-weight:700;"">
                                        NokPortal — Verification Code
                                    </td>
                                    </tr>
                                    <tr><td height=""10""></td></tr>
                                    <tr>
                                    <td style=""font-family:Segoe UI, Arial, sans-serif; color:#334155; font-size:15px; line-height:1.7;"">
                                        Your one-time code is
                                    </td>
                                    </tr>
                                    <tr><td height=""14""></td></tr>
                                    <tr>
                                    <td align=""center"">
                                        <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" border=""0"" style=""margin:auto;"">
                                        <tr>
                                            <td style=""background:#111827; border-radius:12px; padding:16px 24px; border:1px solid #0b1220;"">
                                            <div style=""font-family:Consolas, 'Courier New', Courier, monospace; font-weight:800; font-size:34px; letter-spacing:8px; color:#ffcb0b; text-align:center;"">
                                                {code}
                                            </div>
                                            </td>
                                        </tr>
                                        </table>
                                    </td>
                                    </tr>
                                    <tr><td height=""18""></td></tr>
                                    <tr>
                                    <td style=""font-family:Segoe UI, Arial, sans-serif; color:#475569; font-size:14px; line-height:1.7;"">
                                        This code will expire in <b>{(int)Ttl.TotalMinutes} minutes</b> ( by {expiresAt:yyyy-MM-dd HH:mm} ).
                                    </td>
                                    </tr>
                                    <tr><td height=""14""></td></tr>
                                    <tr>
                                    <td style=""font-family:Segoe UI, Arial, sans-serif; color:#64748b; font-size:12.5px; line-height:1.8;"">
                                        If you didn’t request this, ignore this email. Do not share this code.
                                    </td>
                                    </tr>
                                </table>
                                </td>
                            </tr>
                            <tr>
                                <td style=""background:#0b1220; padding:16px 24px;"">
                                <div style=""font-family:Segoe UI, Arial, sans-serif; color:#a3b3cf; font-size:12px; line-height:1.6; text-align:center;"">
                                    This is an automated message—please do not reply.<br>
                                    © NokPortal. All rights reserved.
                                </div>
                                </td>
                            </tr>
                            </table>
                        </td>
                        </tr>
                    </table>
                    </body>
                    </html>";

            try
            {
                using var client = new SmtpClient("10.93.40.25", 25)
                {
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    EnableSsl = false,
                    UseDefaultCredentials = true,
                    Timeout = 240000
                };
                var from = new MailAddress("NokCare_notifications@nokair.com", "NokPortal");
                using var mail = new MailMessage(from, new MailAddress(emailLower))
                {
                    Subject = subject,
                    Body = htmlBody,
                    IsBodyHtml = true
                };
                mail.Headers.Add("Auto-Submitted", "auto-generated");
                mail.Headers.Add("X-Mailer", "NokPortal");
                await client.SendMailAsync(mail);
            }
            catch (SmtpFailedRecipientsException ex)
            {
                foreach (var inner in ex.InnerExceptions)
                {
                    Console.WriteLine($"Failed recipient: {inner.FailedRecipient}, Reason: {inner.Message}");
                }
            }
            catch (SmtpFailedRecipientException ex)
            {
                Console.WriteLine($"Failed recipient: {ex.FailedRecipient}, Reason: {ex.Message}");
                throw;
            }
            catch (SmtpException ex)
            {
                Console.WriteLine($"SMTP Error: {ex.StatusCode}, Message: {ex.Message}");
                throw;
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Invalid email format: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                throw;
            }
            return new SendOtpResponseDto(VerifyToken: verifyToken, ExpiresAt: expiresAt);
        }

        /// <inheritdoc/>
        public async Task<string> VerifyAsync(VerifyOtpRequestDto req)
        {
            const string FailMessage = "The verification code is invalid or has expired. Please try again.";

            if (string.IsNullOrWhiteSpace(req.VerifyToken) ||
                string.IsNullOrWhiteSpace(req.Email) ||
                string.IsNullOrWhiteSpace(req.Code))
            {
                throw new InvalidOperationException(FailMessage);
            }

            var submittedEmail = req.Email.Trim().ToLowerInvariant();

            var expectedIssuer = string.IsNullOrWhiteSpace(_baseJwt.Issuer) ? "NokAir" : $"{_baseJwt.Issuer}";
            var expectedAudience = string.IsNullOrWhiteSpace(_baseJwt.Audience) ? "NokAir" : $"{_baseJwt.Audience}";

            ClaimsPrincipal? principal;
            try
            {
                principal = _jwtService.DecodeToken(req.VerifyToken!, _baseJwt.SecretKey);
                if (principal is null)
                {
                    throw new InvalidOperationException();
                }
            }
            catch
            {
                throw new InvalidOperationException(FailMessage);
            }

            string? Claim(string type) =>
                principal.Claims.FirstOrDefault(c => string.Equals(c.Type, type, StringComparison.OrdinalIgnoreCase))?.Value;

            if (!string.Equals(Claim("typ"), "otp", StringComparison.Ordinal))
            {
                throw new InvalidOperationException(FailMessage);
            }

            var iss = Claim("iss");
            var aud = Claim("aud");
            if (!string.Equals(iss, expectedIssuer, StringComparison.Ordinal) ||
                !string.Equals(aud, expectedAudience, StringComparison.Ordinal))
            {
                throw new InvalidOperationException(FailMessage);
            }

            var emailInToken = Claim(JwtRegisteredClaimNames.Sub)
                            ?? Claim(ClaimTypes.NameIdentifier)
                            ?? Claim("nameid");
            if (string.IsNullOrWhiteSpace(emailInToken) ||
                !string.Equals(emailInToken.Trim().ToLowerInvariant(), submittedEmail, StringComparison.Ordinal))
            {
                throw new InvalidOperationException(FailMessage);
            }

            var salt = Claim("otp_salt");
            var expectedHash = Claim("otp_hash");
            if (string.IsNullOrEmpty(salt) || string.IsNullOrEmpty(expectedHash))
            {
                throw new InvalidOperationException(FailMessage);
            }

            var digits = NormalizeDigits(req.Code);
            if (digits.Length != 6)
            {
                throw new InvalidOperationException(FailMessage);
            }

            var submittedHash = Sha256($"{digits}:{salt}");
            if (!FixedTimeEqualsHex(submittedHash, expectedHash))
            {
                throw new InvalidOperationException(FailMessage);
            }

            var user = await _userService.GetUserByEmailAsync(emailInToken!);
            if (user is null)
            {
                throw new InvalidOperationException(FailMessage);
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti,       user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email,     user.Email),
                new Claim(JwtRegisteredClaimNames.UniqueName, $"{user.FirstName} {user.LastName}"),
            };

            var token = _jwtService.GenerateJwtTokenInfo(claims);
            return token.Token!;
        }

        /// <inheritdoc/>
        public Task<SendOtpResponseDto> GenerateDecoyAsync(string email)
        {
            var salt = Convert.ToHexString(RandomNumberGenerator.GetBytes(16));
            var fakeHash = Sha256($"{Guid.NewGuid():N}:{salt}");
            var expiresAt = DateTime.UtcNow.Add(Ttl);
            var emailLower = email.Trim().ToLowerInvariant();

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, emailLower),
                new Claim(ClaimTypes.NameIdentifier,   emailLower),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                new Claim("typ", "otp"),
                new Claim("otp_salt",  salt),
                new Claim("otp_hash",  fakeHash),
            };

            var otpJwtSettings = new JwtSettingsModel
            {
                Issuer = string.IsNullOrWhiteSpace(_baseJwt.Issuer) ? "NokPortal.Otp" : $"{_baseJwt.Issuer}.Otp",
                Audience = string.IsNullOrWhiteSpace(_baseJwt.Audience) ? "NokPortal.Auth" : $"{_baseJwt.Audience}.Otp",
                SecretKey = _baseJwt.SecretKey,
                ExpiryTime = expiresAt
            };
            var jwt = _jwtService.GenerateJwtTokenInfo(claims, otpJwtSettings);
            var dto = new SendOtpResponseDto(VerifyToken: jwt.Token!, ExpiresAt: expiresAt);
            return Task.FromResult(dto);
        }

        private static string NormalizeDigits(string s)
            => new string(s.Where(char.IsDigit).ToArray());

        private static bool FixedTimeEqualsHex(string aHex, string bHex)
        {
            var a = Convert.FromHexString(aHex);
            var b = Convert.FromHexString(bHex);
            return CryptographicOperations.FixedTimeEquals(a, b);
        }

        private static string GenerateNumericCode(int len)
        {
            var bytes = RandomNumberGenerator.GetBytes(len);
            var chars = new char[len];
            for (int i = 0; i < len; i++)
            {
                chars[i] = (char)('0' + (bytes[i] % 10));
            }

            return new string(chars);
        }

        /// <summary>
        /// Computes the SHA-256 hash of the given string.
        /// </summary>
        /// <param name="s">The input string.</param>
        /// <returns>The SHA-256 hash as a hexadecimal string.</returns>
        private static string Sha256(string s)
        {
            using var sha = SHA256.Create();
            return Convert.ToHexString(sha.ComputeHash(Encoding.UTF8.GetBytes(s)));
        }
    }
}