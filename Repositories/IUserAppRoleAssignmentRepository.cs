using NokPortalAPI.Models;

namespace NokPortalAPI.Repositories
{
    /// <summary>
    /// Interface for the AssignedUserAppRoleRepository.
    /// </summary>
    public interface IUserAppRoleAssignmentRepository
    {
        /// <summary>
        /// Adds an user app role assignment asynchronously.
        /// </summary>
        /// <param name="assignedUserAppRole">User app role assignment to add.</param>
        /// <returns>Created user app role assignment.</returns>
        Task<UserAppRoleAssignment> AddUserAppRoleAssignmentAsync(UserAppRoleAssignment assignedUserAppRole);

        /// <summary>
        /// Deletes an user app role assignment by ID asynchronously.
        /// </summary>
        /// <param name="id">The ID of the user app role assignment to delete.</param>
        /// <returns>Number of rows affected.</returns>
        Task<int> DeleteUserAppRoleAssignmentByIdAsync(int id);

        /// <summary>
        /// Gets an user app role assignment by ID asynchronously.
        /// </summary>
        /// <param name="id">The ID of the user app role assignment to get.</param>
        /// <returns>Returns the user app role assignment if found; otherwise, null.</returns>
        Task<UserAppRoleAssignment?> GetUserAppRoleAssignmentByIdAsync(int id);

        /// <summary>
        /// Checks if a user app role exists asynchronously.
        /// </summary>
        /// <param name="userId">User ID to check.</param>
        /// <param name="appId">App ID to check.</param>
        /// <param name="roleId">Role ID to check.</param>
        /// <returns>True if the user app role exists; otherwise, false.</returns>
        Task<bool> IsUserAppRoleExistAsync(int userId, int appId, int roleId);

        /// <summary>
        /// Checks if a user is assigned to an app asynchronously.
        /// </summary>
        /// <param name="userId">User ID to check.</param>
        /// <param name="appId">App ID to check.</param>
        /// <returns>True if the user is assigned to the app; otherwise, false.</returns>
        Task<bool> IsUserAssignedToAppAsync(int userId, int appId);
    }
}
