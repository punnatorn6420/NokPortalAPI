using NokCore.Api.JWT.Models;
using NokCore.Identity.Models;
using NokPortalAPI.Domains.Models;

namespace NokPortalAPI.Domains.Services
{
    public interface IUserAppsService
    {
        Task AddUserAppAsync(int appId, RequestAddUserApp addUser);

        Task<ModelUserApp> GetUserAppAsync(int appId, int userId);

        Task<bool> CheckRoleLevelAsync(EnumUserRole roleLevel, int appId, int userId);

        Task<IEnumerable<App>> GetUserAllAppAsync(int userId);

        Task<(AppEnv, ResponseJwt)> GetJWTTokenTargetApp(int appId, EnumEnvironmentType env);

        Task<(AppEnv, ResponseJwt)> GetJWTTokenTargetAppWithData(int appId, EnumEnvironmentType env, User user, IEnumerable<UserApps> userAppsList);
    }
}
