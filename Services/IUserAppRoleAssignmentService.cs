using NokAir.Shared.Security.Models.Common;
using NokPortalAPI.Dtos;

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
        /// <returns>True if the user is assigned to the app, otherwise false.</returns>
        Task<bool> AssignUserToAppAsync(UserAppAssignmentRequest userAppAssignmentReq);

        /// <summary>
        /// Gets the JWT token info by user and app.
        /// </summary>
        /// <param name="userId">User ID.</param>
        /// <param name="appId">Application ID.</param>
        /// <returns>JWT token info.</returns>
        Task<JwtInfoModel> GetJwtTokenInfoByUserAppAsync(int userId, int appId);

        /// <summary>
        /// Gets list of roles the environment of the target app.
        /// </summary>
        /// <param name="appId">Application ID.</param>
        /// <returns>List of roles.</returns>
        Task<IList<RoleDto>> GetRolesByAppIdAsync(int appId);

        /// <summary>
        /// Gets user assigned role IDs for a specific app.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="appId">The application ID.</param>
        /// <returns>User app role information.</returns>
        Task<UserAppRoleDto> GetUserAppRoleAsync(int userId, int appId);
    }
}