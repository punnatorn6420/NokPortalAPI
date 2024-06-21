using NokPortal.Domains.Models;

namespace NokPortal.Domians.Services
{
    public interface IUserAppsService
    {
        Task<int> AddUserAppAsync(UserApp userApp);

        Task<UserApp> GetUserAppAsync(int appId, int userId);
    }
}
