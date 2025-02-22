using NokPortalAPI.Models;

namespace NokPortalAPI.Services.Old
{
    public interface IAppEnvService
    {
        Task<bool> CreateAppEnvAsync(AppEnvironment appEnv);

        Task<bool> UpdateAppEnvAsync(AppEnvironment appEnv);

        Task<IEnumerable<AppEnvironment>> GetAllAppEnvAsync();

        Task<AppEnvironment?> GetById(int appId, int userId, EnumEnvironmentType env);
    }
}