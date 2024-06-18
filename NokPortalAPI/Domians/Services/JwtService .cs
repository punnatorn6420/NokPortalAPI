using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using NokPortal.Domians.Models;

namespace NokPortal.Domians.Services
{
    public class JwtService : IJwtService
    {
        // private readonly IConfiguration _configuration;
        private readonly string _secretKey;
        private readonly int _hourExpire;

        private readonly string _issuer = "nokdev";
        private readonly string _audience = "nokdev";

        public JwtService(string secretKey, int hourExpire)
        {
            _secretKey = secretKey;
            _hourExpire = hourExpire;
        }

        public dynamic GenerateToken(dynamic jwtSetting)
        {
            JsonHelperService jsonHelper = new JsonHelperService();

            var claims = new List<Claim>();
            foreach (var property in jwtSetting.GetType().GetProperties())
            {
                var propertyName = property.Name;
                var propertyValue = property.GetValue(jwtSetting)?.ToString() ?? string.Empty;
                claims.Add(new Claim(propertyName, propertyValue));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            DateTime expiresTime = DateTime.Now.AddHours(_hourExpire);

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
            var key = Encoding.UTF8.GetBytes(_secretKey);
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
