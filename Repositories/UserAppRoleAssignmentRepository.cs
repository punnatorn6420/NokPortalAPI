using Microsoft.EntityFrameworkCore;
using NokPortalAPI.Models;

namespace NokPortalAPI.Repositories
{
    public class UserAppRoleAssignmentRepository : IUserAppRoleAssignmentRepository
    {
        private readonly AppDbContext context;

        public UserAppRoleAssignmentRepository(AppDbContext context)
        {
            this.context = context;
        }

        /// <inheritdoc/>
        public async Task<UserAppRoleAssignment> AddUserAppRoleAssignmentAsync(UserAppRoleAssignment assignedUserAppRole)
        {
            await context.UserAppRoleAssignments.AddAsync(assignedUserAppRole);
            await context.SaveChangesAsync();
            return assignedUserAppRole;
        }

        /// <inheritdoc/>
        public async Task<int> DeleteUserAppRoleAssignmentByIdAsync(int id)
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
        public async Task<UserAppRoleAssignment?> GetUserAppRoleAssignmentByIdAsync(int id)
        {
            return await context.UserAppRoleAssignments.FindAsync(id);
        }

        /// <inheritdoc/>
        public Task<bool> IsUserAppRoleExistAsync(int userId, int appId, int roleId)
        {
            return context.UserAppRoleAssignments
                .AnyAsync(x => x.UserId == userId && x.AppId == appId && x.RoleId == roleId);
        }

        /// <inheritdoc/>
        public async Task<bool> IsUserAssignedToAppAsync(int userId, int appId)
        {
            return await context.UserAppRoleAssignments
                .AnyAsync(x => x.UserId == userId && x.AppId == appId);
        }
    }
}
