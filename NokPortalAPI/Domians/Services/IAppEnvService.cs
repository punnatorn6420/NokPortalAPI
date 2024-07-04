using System.Data;
using NokPortal.Domians.Models;
using NokPortalAPI.Domians.Models;

namespace NokPortal.Domians.Services
{
    public interface IAppEnvService
    {
        Task<bool> CreateAppsEnv(AppEnv reqCreateAppEnv, int appId);

        Task<bool> UpdateAppsEnv(AppEnv reqCreateAppEnv, int appId);

        Task<AppEnv> GetById(int appId, int userId, EnumEnvironmentType env);
    }
}