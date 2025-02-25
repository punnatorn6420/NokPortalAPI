using NokCore.Identity.Models;
using NokPortalAPI.Models;

namespace NokPortalAPI.Services
{
    /// <summary>
    /// Interface for the target app service.
    /// </summary>
    public interface ITargetAppService
    {
        /// <summary>
        /// Gets list of roles the environment of the target app.
        /// </summary>
        /// <param name="appId">Application ID.</param>
        /// <param name="env">Environment type.</param>
        /// <returns>List of roles.</returns>
        Task<IList<Role>> GetAppRolesByAppIdAndEnvAsync(int appId, EnumEnvironmentType env);
    }
}
