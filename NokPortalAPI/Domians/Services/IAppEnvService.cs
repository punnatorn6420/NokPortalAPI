using System.Data;
using NokPortalAPI.Domains.Models;
using NokPortalAPI.Domains.Models;

namespace NokPortalAPI.Domains.Services
{
    public interface IAppEnvService
    {
        Task<bool> CreateAppsEnv(AppEnv reqCreateAppEnv, int appId);

        Task<bool> UpdateAppsEnv(AppEnv reqCreateAppEnv, int appId);

        Task<ResponseAppEnv> GetById(int appId, int userId, EnumEnvironmentType env);
    }
}