using NokCore.Identity.Models;
using NokPortalAPI.Domains.Models;

namespace NokPortalAPI.Domains.Services
{
    public interface IAppService
    {
        Task<bool> CreateAppAsync(App app);

        Task<bool> UpdateAppAsync(App app);

        Task<IEnumerable<App>> GetAllAppsAsync();

        Task<App> GetAppByIdAsync(int appId);

        // Task<ResponseJwt> AssignUserTargetAppAsync(int appId, int userId);
        Task<IEnumerable<Role>> GetAppTargetAllRole(int appId, EnumEnvironmentType env);

        Task<ResponseAppLink> GetAppLink(int appId, int userId, EnumEnvironmentType env);
    }
}
