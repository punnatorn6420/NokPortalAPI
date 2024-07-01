using NokPortal.Domains.Models;
using NokPortal.Domians.Models;
using NokPortalAPI.Domians.Models;

namespace NokPortal.Domians.Services
{
    public interface IUserAppsService
    {
        Task<int> AddUserAppAsync(ModelUserApp userApp);

        Task<ModelUserApp> GetUserAppAsync(int appId, int userId);

        Task<bool> CheckRoleLevelAsync(EnumUserRole roleLevel, int appId, int userId);

        Task<IEnumerable<ModelApp>> GetUserAllAppAsync(int userId);
    }
}
