using NokPortalAPI.Entities;

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

        /// <summary>
        ///  Gets user role IDs for a specific app asynchronously.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="appId">The application ID.</param>
        /// <returns>A list of role IDs.</returns>
        Task<List<int>> GetUserRoleIdsForAppAsync(int userId, int appId);

        /// <summary>
        /// Gets user role IDs by user ID and app ID.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="appId">The application ID.</param>
        /// <returns>A list of role IDs.</returns>
        Task<IList<int>> GetUserRoleIdsByUserAndAppAsync(int userId, int appId);

        /// <summary>
        /// Deletes all user app role assignments for a specific user and app.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="appId">The application ID.</param>
        /// <returns>Number of deleted records.</returns>
        Task<int> DeleteAllUserAppRoleAssignmentsByUserAndAppAsync(int userId, int appId);
    }
}