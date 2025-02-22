using NokCore.Identity.Models;

namespace NokPortalAPI.Repositories
{
    /// <summary>
    /// Interface for role repository.
    /// </summary>
    public interface IRoleRepository
    {
        /// <summary>
        /// Add new role.
        /// </summary>
        /// <param name="role">Role to add.</param>
        /// <returns>Added role.</returns>
        Task<Role> AddRoleAsync(Role role);

        /// <summary>
        /// Delete role by ID.
        /// </summary>
        /// <param name="id">Role ID.</param>
        /// <returns>Number of rows affected.</returns>
        Task<int> DeleteRoleByIdAsync(int id);

        /// <summary>
        /// Get role by ID.
        /// </summary>
        /// <param name="id">Role ID.</param>
        /// <returns>Role or null if not found.</returns>
        Task<Role?> GetRoleByIdAsync(int id);

        /// <summary>
        /// Get roles by search criteria.
        /// </summary>
        /// <param name="searchCriteria">Search criteria.</param>
        /// <returns>List of roles.</returns>
        Task<IList<Role>> GetRolesByCriteriaAsync(RoleSearchCriteria searchCriteria);

        /// <summary>
        /// Update role.
        /// </summary>
        /// <param name="role">Role to update.</param>
        /// <returns>Number of rows affected.</returns>
        Task<int> UpdateRoleAsync(Role role);
    }
}
