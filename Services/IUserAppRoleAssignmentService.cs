using NokCore.Api.JWT.Models;
using NokPortalAPI.Models;

namespace NokPortalAPI.Services
{
    /// <summary>
    /// Interface for the managed app service.
    /// </summary>
    public interface IUserAppRoleAssignmentService
    {
        /// <summary>
        /// Assigns user to an app.
        /// </summary>
        /// <param name="userAppRoleAssignment">User app role assignment request.</param>
        /// <returns>True if the user is assigned to the app, otherwise false.</returns>
        Task<bool> AssignUserToAppAsync(UserAppAssignmentRequest userAppAssignmentReq);

        /// <summary>
        /// Gets the JWT token info by user and app.
        /// </summary>
        /// <param name="userId">User ID.</param>
        /// <param name="appId">Application ID.</param>
        /// <returns>JWT token info.</returns>
        Task<JwtTokenInfo> GetJwtTokenInfoByUserAppAsync(int userId, int appId);

        /// <summary>
        /// Gets list of roles the environment of the target app.
        /// </summary>
        /// <param name="appId">Application ID.</param>
        /// <returns>List of roles.</returns>
        Task<IList<Role>> GetRolesByAppIdAsync(int appId);
    }
}
