using NokCore.Identity.Models;
using NokPortal.Domians.Models;

namespace NokPortal.Domains.Services
{
    public interface IAuthService
    {
        Task<User> GetUsersByIdAsync(int userID);
    }
}
