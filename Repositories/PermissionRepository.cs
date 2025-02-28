using Microsoft.EntityFrameworkCore;

namespace NokPortalAPI.Repositories
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly AppDbContext context;

        public PermissionRepository(AppDbContext context)
        {
            this.context = context;
        }

        /// <inheritdoc/>
        public async Task<bool> HasUserPermissionAsync(int userId, string permission)
        {
            return await context.UserRoles
                .AnyAsync(x => x.UserId == userId && x.Role.RolePermissions
                    .Any(p => p.Permission.Name == permission));
        }

        /// <inheritdoc/>
        public async Task<bool> HasUserPermissionsAsync(int userId, string[] permissions)
        {
            return await context.UserRoles
                .AnyAsync(x => x.UserId == userId && x.Role.RolePermissions
                    .Any(p => permissions.Contains(p.Permission.Name)));
        }
    }
}
