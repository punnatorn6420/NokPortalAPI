using Microsoft.EntityFrameworkCore;
using NokPortalAPI.Entities;

namespace NokPortalAPI.Repositories
{
    /// <summary>
    /// User App Role Assignment Repository
    /// </summary>
    public class UserAppRoleAssignmentRepository : IUserAppRoleAssignmentRepository
    {
        private readonly AppDbContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserAppRoleAssignmentRepository"/> class.
        /// </summary>
        /// <param name="context"></param>
        public UserAppRoleAssignmentRepository(AppDbContext context)
        {
            this.context = context;
        }

        /// <inheritdoc/>
        public async Task<UserAppRoleAssignment> AddUserAppRoleAssignmentAsync(UserAppRoleAssignment assignedUserAppRole)
        {
            await context.UserAppRoleAssignments.AddAsync(assignedUserAppRole);
            return assignedUserAppRole;
        }

        /// <inheritdoc/>
        public async Task<int> RemoveUserAppRoleAssignmentAsync(int id)
        {
            var assignedUserAppRole = await context.UserAppRoleAssignments.FindAsync(id);
            if (assignedUserAppRole == null)
            {
                return 0;
            }

            context.UserAppRoleAssignments.Remove(assignedUserAppRole);
            return await context.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task<UserAppRoleAssignment?> FindUserAppRoleAssignmentAsync(int id)
        {
            return await context.UserAppRoleAssignments.FindAsync(id);
        }

        /// <inheritdoc/>
        public Task<bool> ExistsAsync(int userId, int appId, int roleId)
        {
            return context.UserAppRoleAssignments
                .AnyAsync(x => x.UserId == userId && x.AppId == appId && x.RoleId == roleId);
        }

        /// <inheritdoc/>
        public Task<List<int>> FindRoleIdsAsync(int userId, int appId)
        {
            return context.UserAppRoleAssignments
                .Where(x => x.UserId == userId && x.AppId == appId)
                .Select(x => x.RoleId)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<bool> ExistsAssignmentAsync(int userId, int appId)
        {
            return await context.UserAppRoleAssignments
                .AnyAsync(x => x.UserId == userId && x.AppId == appId);
        }

        /// <inheritdoc/>
        public async Task<IList<int>> FindRoleIdsByAppAsync(int userId, int appId)
        {
            return await context.UserAppRoleAssignments
                .Where(x => x.UserId == userId && x.AppId == appId)
                .Select(x => x.RoleId)
                .OrderBy(x => x)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<int> RemoveUserAppRoleAssignmentsAsync(int userId, int appId)
        {
            var existingAssignments = await context.UserAppRoleAssignments
                .Where(x => x.UserId == userId && x.AppId == appId)
                .ToListAsync();

            if (existingAssignments.Any())
            {
                context.UserAppRoleAssignments.RemoveRange(existingAssignments);
                return await context.SaveChangesAsync();
            }

            return 0;
        }
    }
}