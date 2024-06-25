using NokPortal.Domians.Models;

namespace NokPortal.Domians.Services
{
    public interface IAppService
    {
        Task CreateAppAsync(RequestCreateApp reqCreate);

        Task<IEnumerable<App>> GetAllAppAsync();

        Task<App> GetAppByIdAsync(int appId);

        Task<string> GenerateJwtTargetApp(int appId);
    }
}
