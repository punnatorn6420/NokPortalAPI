using NokPortal.Domains.Models;

namespace NokPortal.Domians.Services
{
    public interface IUsersAppsService
    {
        Task<int> AddUserAppAsync(UserApp userApp);
    }
}
