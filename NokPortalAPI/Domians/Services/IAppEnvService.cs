using System.Data;
using NokPortal.Domians.Models;

namespace NokPortal.Domians.Services
{
    public interface IAppEnvService
    {
        Task<bool> CreateAppsEnv(RequestCreateAppEnv reqCreateAppEnv, int appId);

        Task<RequestCreateAppEnv> GetById(int appId, int userId);
    }
}