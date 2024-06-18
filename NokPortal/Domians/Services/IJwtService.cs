using System.Security.Claims;
using NokPortal.Domians.Models;

namespace NokPortal.Domians.Services
{
    public interface IJwtService
    {
        ResponseJwt GenerateToken(JWTsetting jwtSetting);

        ClaimsPrincipal? DecodeToken(string token);
    }
}
