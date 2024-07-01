using System.Data;
using NokPortal.Domians.Models;

namespace NokPortal.Domians.Services
{
    public interface IAppEnvService
    {
        Task<bool> CreateAppsEnv(AppEnv reqCreateAppEnv, int appId);

        Task<IEnumerable<AppEnv>> GetById(int appId, int userId);
    }
}