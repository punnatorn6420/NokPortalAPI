using NokCore.Identity.Models;
using NokPortal.Domians.Models;
using NokPortalAPI.Domians.Models;

namespace NokPortal.Domians.Services
{
    public interface IAppService
    {
        Task<bool> CreateAppAsync(RequestApp reqCreate);

        Task<bool> UpdateAppAsync(RequestApp reqUpdate);

        Task<IEnumerable<ModelApp>> GetAllAppAsync();

        Task<ModelApp> GetAppByIdAsync(int appId);

        // Task<ResponseJwt> AssignUserTargetAppAsync(int appId, int userId);
        Task<IEnumerable<Role>> GetAppTargetAllRole(int appId, EnumEnvironmentType env);

        Task<ResponseAppLink> GetAppLink(int appId, int userId, EnumEnvironmentType env);
    }
}
