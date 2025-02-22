using NokCore.Api.JWT.Models;
using NokCore.Identity.Models;
using NokPortalAPI.Models;

namespace NokPortalAPI.Services.Old
{
    public interface IUserAppsService
    {
        Task AddUserAppAsync(int appId, RequestAddUserApp addUser);

        Task<ModelUserApp> GetUserAppAsync(int appId, int userId);

        Task<bool> CheckRoleLevelAsync(EnumUserRole roleLevel, int appId, int userId);

        Task<IEnumerable<App>> GetAllAppsByUserIdAsync(int userId);

        Task<(AppEnvironment, JwtResponse)> GetJWTForTargetAppAsync(int appId, EnumEnvironmentType env);

        Task<(AppEnvironment, JwtResponse)> GetJWTTokenTargetAppWithData(int appId, EnumEnvironmentType env, User user, IEnumerable<AppWithRoles> userAppsList);

    }
}
