using System.Security.Claims;
using NokPortal.Domians.Models;

namespace NokPortal.Domians.Services
{
    public interface IJwtService
    {
        dynamic GenerateToken(dynamic jwtSetting);

        ClaimsPrincipal? DecodeToken(string token);
    }
}
