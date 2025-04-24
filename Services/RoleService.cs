
using NokPortalAPI.Entities;
using NokPortalAPI.Repositories;

namespace NokPortalAPI.Services
{
    /// <summary>
    /// Permission service.
    /// </summary>
    public class RoleService : IRoleService
    {
        private readonly AppDbContext context;
        private readonly IRoleRepository<Role> roleRepository;

        public RoleService(AppDbContext context, IRoleRepository<Role> roleRepository)
        {
            this.context = context;
            this.roleRepository = roleRepository;
        }

        /// <inheritdoc />
        public async Task<bool> IsUserInRoleAsync(int userId, string permission)
        {
            return await roleRepository.IsUserInRoleAsync(userId, permission);
        }

        /// <inheritdoc />
        public async Task<bool> IsUserInRolesAsync(int userId, string[] permissions)
        {
            return await roleRepository.IsUserInRolesAsync(userId, permissions);
        }
    }
}
