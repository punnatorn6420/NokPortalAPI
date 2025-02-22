using NokCore.Identity.Models;
using NokPortalAPI.Models;

namespace NokPortalAPI.Services.Old
{
    public interface IAppService
    {
        Task<bool> CreateAppAsync(App app);

        Task<bool> UpdateAppAsync(App app);

        Task<IList<App>> GetAllAppsAsync();

        Task<App> GetAppByIdAsync(int appId);

        // Task<ResponseJwt> AssignUserTargetAppAsync(int appId, int userId);
        Task<IList<Role>> GetAppTargetAllRole(int appId, EnumEnvironmentType env);

        Task<ResponseAppLink> GetAppLink(int appId, int userId, EnumEnvironmentType env);
    }
}
