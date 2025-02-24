using Microsoft.EntityFrameworkCore;
using NokCore.Identity.Models;
using NokPortalAPI.Models;
using System.Linq.Dynamic.Core;

namespace NokPortalAPI.Repositories
{
    /// <summary>
    /// User repository.
    /// </summary>
    public class UserRepository : IUserRepository<User>
    {
        private readonly AppDbContext context;

        public UserRepository(AppDbContext context)
        {
            this.context = context;
        }

        /// <inheritdoc />
        public async Task<User> AddUserAsync(User user)
        {
            var result = await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
            return result.Entity;
        }

        /// <inheritdoc />
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await context.Users
               .Include(u => u.UserRoles)
                   .ThenInclude(ur => ur.Role)
               .FirstOrDefaultAsync(u => u.Email == email);
        }

        /// <inheritdoc />
        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        /// <inheritdoc />
        public async Task<ICollection<User>> GetUsersByAppIdAsync(int appId)
        {
            // Get users by application ID from AssignedUserApps table.
            var query = from aua in context.AssignedUserApps
                        join u in context.Users on aua.UserId equals u.Id
                        where aua.AppId == appId
                        select u;

            return await query.ToListAsync();
        }

        /// <inheritdoc />
        public async Task<ICollection<User>> GetUsersByCriteriaAsync(UserSearchCriteria searchCriteria)
        {
            IQueryable<User> query = context.Users;

            if (!string.IsNullOrEmpty(searchCriteria.Keyword))
            {
                query = query.Where(a => a.FirstName.Contains(searchCriteria.Keyword) || a.LastName.Contains(searchCriteria.Keyword));
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

        /// <inheritdoc />
        public async Task<int> UpdateUserAsync(User user)
        {
            context.Users.Update(user);
            return await context.SaveChangesAsync();
        }
    }
}
