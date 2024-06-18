using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using NokPortal.Domians.Models;

namespace NokPortal.Domians.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        private readonly string secretKey = "secret123456789abcdefghigklmnopqrst";
        private readonly string _issuer = "NokPortalAPI"; //nokdev
        private readonly string _audience = "WebPortal"; //nokdev

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public ResponseJwt GenerateToken(JWTsetting jwtSetting)
        {
            JsonHelperService jsonHelper = new JsonHelperService();

            var claims = new List<Claim>
            {
                new Claim(jsonHelper.GetJsonPropertyName<JWTsetting>(nameof(JWTsetting.UserId)), jwtSetting.UserId.ToString())
            };

            if (jwtSetting.ApplicationId != null)
            {
                claims.Add(new Claim(jsonHelper.GetJsonPropertyName<JWTsetting>(nameof(JWTsetting.ApplicationId)), jwtSetting.ApplicationId.ToString() ?? throw new ArgumentNullException("ApplicationId is missing")));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            DateTime expiresTime = DateTime.Now.AddHours(24);

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: expiresTime,
                signingCredentials: creds);

            return new ResponseJwt
            {
                ExpiresTime = expiresTime,
                Token = new JwtSecurityTokenHandler().WriteToken(token)
            };
        }

        public ClaimsPrincipal? DecodeToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(secretKey);
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _issuer,
                ValidAudience = _audience,
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                return principal;
            }
            catch
            {
                return null;
            }
        }
    }
}
