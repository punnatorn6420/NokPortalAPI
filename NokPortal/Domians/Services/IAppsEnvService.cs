using System.Data;
using NokPortal.Domians.Models;

namespace NokPortal.Domians.Services
{
    public interface IAppsEnvService
    {
        Task<bool> CreateAppsEnv(AppsEnvModel model);

        Task<AppsEnvModel> GetById(RequestAppInfo app, int userId);
    }
}