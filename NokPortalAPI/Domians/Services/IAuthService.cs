using NokPortal.Domians.Models;

namespace NokPortal.Domains.Services
{
    public interface IAuthService
    {
        Task<Users> GetUsersByIdAsync(int userID);
    }
}
