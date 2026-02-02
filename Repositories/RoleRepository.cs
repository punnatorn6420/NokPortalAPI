using Microsoft.EntityFrameworkCore;
using NokAir.Core.Abstractions.Entities.Rbac;
using NokPortalAPI.Entities;
using System.Linq.Dynamic.Core;
using System.Threading;

namespace NokPortalAPI.Repositories
{
    /// <summary>
    /// Role repository.
    /// </summary>
    public class RoleRepository : IRoleRepository<Role>
    {
        private readonly AppDbContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoleRepository"/> class.
        /// </summary>
        /// <param name="context"></param>
        public RoleRepository(AppDbContext context)
        {
            this.context = context;
        }

        /// <inheritdoc/>
        public async Task<Role> AddRoleAsync(Role role, CancellationToken cancellationToken = default)
        {
            var result = await context.Roles.AddAsync(role);
            await context.SaveChangesAsync();
            return result.Entity;
        }

        /// <inheritdoc/>
        public async Task<int> RemoveRoleByIdAsync(int roleId, CancellationToken cancellationToken = default)
        {
            var role = await context.Roles.FindAsync(roleId);
            if (role == null)
            {
                return 0;
            }

            context.Roles.Remove(role);
            return await context.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task<ICollection<Role>> FindRolesByCriteriaAsync(IRoleSearchCriteria searchCriteria, CancellationToken cancellationToken = default)
        {
            IQueryable<Role> query = context.Roles;
            if (!string.IsNullOrEmpty(searchCriteria.Keyword))
            {
                query = query.Where(r => r.Name.Contains(searchCriteria.Keyword));
            }

            if (!string.IsNullOrEmpty(searchCriteria.SortField))
            {
                var sortDirection = searchCriteria.Ascending ? "ascending" : "descending";
                query = query.OrderBy($"{searchCriteria.SortField} {sortDirection}");
            }

            return await query
                .Skip((searchCriteria.PageNumber - 1) * searchCriteria.PageSize)
                .Take(searchCriteria.PageSize)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<bool> ExistsUserInRoleAsync(int userId, string requiredRole, CancellationToken cancellationToken = default)
        {
            return await context.UserRoles
                .AnyAsync(ur => ur.UserId == userId && ur.Role!.Name == requiredRole);
        }

        /// <inheritdoc/>
        public async Task<bool> ExistsUserInRolesAsync(int userId, string[] requiredRoles, CancellationToken cancellationToken = default)
        {
            var userRoles = await context.UserRoles
                 .Where(ur => ur.UserId == userId)
                 .Select(ur => ur.Role.Name)
                 .ToListAsync();

            return userRoles.Any(r => requiredRoles.Contains(r));
        }

        /// <inheritdoc/>
        public async Task<int> UpdateRoleAsync(Role role, CancellationToken cancellationToken = default)
        {
            context.Roles.Update(role);
            return await context.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task AddUserRoleAsync(int userId, int defaultRoleId = 3, CancellationToken cancellationToken = default)
        {
            if (defaultRoleId <= 0)
            {
                throw new ArgumentException("Invalid default role ID");
            }

            var exists = await context.Roles.AnyAsync(r => r.Id == defaultRoleId);
            if (!exists)
            {
                throw new Exception($"Role ID {defaultRoleId} does not exist");
            }

            var userRole = new UserRole
            {
                UserId = userId,
                RoleId = defaultRoleId,
            };
            await context.UserRoles.AddAsync(userRole);
            await context.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<IRole>> FindAllRolesAsync(CancellationToken cancellationToken)
        {
            return await context.Roles
                .AsNoTracking()
                .Cast<IRole>()
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<Role?> FindByIdAsync(int roleId, CancellationToken cancellationToken = default)
        {
            return await context.Roles.FindAsync(roleId);
        }
    }
}