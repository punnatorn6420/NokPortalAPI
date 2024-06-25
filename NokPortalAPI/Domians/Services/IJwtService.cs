using System.Security.Claims;
using NokPortal.Domians.Models;

namespace NokPortal.Domians.Services
{
    public interface IJwtService
    {
        dynamic GenerateToken(dynamic jwtSetting);

        ClaimsPrincipal? DecodeToken(string token);

        dynamic GenerateToken(dynamic jwtSetting, string secretKey, int hourExpire);

        ClaimsPrincipal? DecodeToken(string token, string secretKey);
    }
}
