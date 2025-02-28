
using NokPortalAPI.Repositories;

namespace NokPortalAPI.Services
{
    /// <summary>
    /// Permission service.
    /// </summary>
    public class PermissionService : IPermissionService
    {
        private readonly AppDbContext context;
        private readonly IPermissionRepository permissionRepository;

        public PermissionService(AppDbContext context, IPermissionRepository permissionRepository)
        {
            this.context = context;
            this.permissionRepository = permissionRepository;
        }

        /// <inheritdoc />
        public async Task<bool> HasUserPermissionAsync(int userId, string permission)
        {
            return await permissionRepository.HasUserPermissionAsync(userId, permission);
        }

        /// <inheritdoc />
        public async Task<bool> HasUserPermissionsAsync(int userId, string[] permissions)
        {
            return await permissionRepository.HasUserPermissionsAsync(userId, permissions);
        }
    }
}
