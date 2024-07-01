using NokCore.Api.JwtToken.Models;
using NokCore.Identity.Models;
using NokPortal.Domians.Models;

namespace NokPortal.Domians.Services
{
    public interface IAppService
    {
        Task<bool> CreateAppAsync(RequestCreateApp reqCreate);

        Task<IEnumerable<ModelApp>> GetAllAppAsync();

        Task<ModelApp> GetAppByIdAsync(int appId);

        Task<ResponseJwt> AssignUserTargetAppAsync(int appId, int userId);

        Task<IEnumerable<Role>> GetAppTargetAllRole(int appId);
    }
}
