using NokAir.Core.Abstractions.Entities.Rbac;
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

        /// <summary>
        /// Initializes a new instance of the <see cref="RoleService"/> class.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="roleRepository"></param>
        public RoleService(AppDbContext context, IRoleRepository<Role> roleRepository)
        {
            this.context = context;
            this.roleRepository = roleRepository;
        }

        /// <inheritdoc />
        public async Task<bool> IsUserInRoleAsync(int userId, string permission, CancellationToken cancellationToken)
        {
            return await roleRepository.ExistsUserInRoleAsync(userId, permission);
        }

        /// <inheritdoc />
        public async Task<bool> IsUserInRolesAsync(int userId, string[] permissions, CancellationToken cancellationToken)
        {
            return await roleRepository.ExistsUserInRolesAsync(userId, permissions);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<IRole>> GetAllRolesAsync(CancellationToken cancellationToken)
        {
            return await roleRepository.FindAllRolesAsync();
        }
    }
}