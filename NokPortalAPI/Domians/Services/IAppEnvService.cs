using NokPortalAPI.Domains.Models;

namespace NokPortalAPI.Domains.Services
{
    public interface IAppEnvService
    {
        Task<bool> CreateAppEnvAsync(AppEnv appEnv);

        Task<bool> UpdateAppEnvAsync(AppEnv appEnv);

        Task<IEnumerable<AppEnv>> GetAllAppEnvAsync();

        Task<AppEnv?> GetById(int appId, int userId, EnumEnvironmentType env);
    }
}