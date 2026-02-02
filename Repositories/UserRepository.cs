using Microsoft.EntityFrameworkCore;
using NokPortalAPI.Dtos;
using NokPortalAPI.Entities;
using System.Linq.Dynamic.Core;

namespace NokPortalAPI.Repositories
{
    /// <summary>
    /// User repository.
    /// </summary>
    public class UserRepository : IUserRepository<User>
    {
        private readonly AppDbContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserRepository"/> class.
        /// </summary>
        /// <param name="context"></param>
        public UserRepository(AppDbContext context)
        {
            this.context = context;
        }

        /// <inheritdoc />
        public async Task<User> AddUserAsync(User user, CancellationToken cancellationToken = default)
        {
            var result = await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
            return result.Entity;
        }

        /// <inheritdoc/>
        public async Task<MyProfileDto?> FindMyProfileAsync(int userId, CancellationToken cancellationToken)
        {
            var user = await context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .Include(u => u.UserAppRoleAssignments)
                    .ThenInclude(ua => ua.App)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return null;
            }

            var apps = user.UserAppRoleAssignments
                .Where(ua => ua.App != null)
                .Select(ua => ua.App!)
                .DistinctBy(a => a.Id)
                .Select(app => new AppSummaryDto
                {
                    Id = app.Id,
                    Name = app.Name,
                    Header = app.Header,
                    Subheader = app.Subheader,
                    ClientUrl = app.ClientUrl,
                    BackendUrl = app.BackendUrl,
                    ImageUrl = app.ImageUrl,
                    Remark = app.Remark,
                    JwtExpiryHours = app.JwtExpiryHours,
                    EnvironmentType = app.EnvironmentType,
                    Active = app.Active
                })
                .ToList();

            var roleId = user.UserRoles
                .Select(ur => ur.Role!.Id)
                .FirstOrDefault().ToString() ?? string.Empty;

            var result = new MyProfileDto
            {
                Id = user.Id,
                ObjectId = user.ObjectId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                JobTitle = user.JobTitle,
                Department = user.Department,
                Role = roleId,
                Active = user.Active,
                Apps = apps
            };

            return result;
        }

        /// <inheritdoc />
        public async Task<User?> FindUserByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            var norm = email.Trim();
            return await context.Users
                .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => EF.Functions.ILike(u.Email, norm));
        }

        /// <inheritdoc />
        public async Task<User?> FindUserByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .Include(u => u.UserAppRoleAssignments)
                    .ThenInclude(ua => ua.App)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        /// <inheritdoc />
        public async Task<ICollection<User>> FindUsersByAppIdAsync(int appId, CancellationToken cancellationToken)
        {
            // Get users by application ID from AssignedUserApps table.
            var query = from aua in context.UserAppRoleAssignments
                        join u in context.Users on aua.UserId equals u.Id
                        where aua.AppId == appId
                        select u;

            return await query.ToListAsync();
        }

        /// <inheritdoc />
        public async Task<(IList<User> Items, int TotalRecords)> FindUsersByCriteriaAsync(UserSearchCriteria criteria, CancellationToken cancellationToken)
        {
            IQueryable<User> query = context.Users
                .Include(u => u.UserRoles)
                .AsNoTracking();
            var keyword = criteria.Keyword?.Trim();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var pattern = $"%{keyword}%";
                query = query.Where(u =>
                    (u.FirstName != null && EF.Functions.ILike(u.FirstName, pattern)) ||
                    (u.LastName != null && EF.Functions.ILike(u.LastName, pattern)) ||
                    (u.Email != null && EF.Functions.ILike(u.Email, pattern)));
            }

            var total = await query.CountAsync();
            var allowed = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            { "Id", "FirstName", "LastName", "Email", "CreatedAt" };
            var field = string.IsNullOrWhiteSpace(criteria.SortField) ? "Id" : criteria.SortField.Trim();
            if (!allowed.Contains(field))
            {
                field = "Id";
            }

            var dir = criteria.Ascending ? "ascending" : "descending";
            query = query.OrderBy($"{field} {dir}");
            var pageNumber = Math.Max(1, criteria.PageNumber);
            var pageSize = Math.Clamp(criteria.PageSize, 1, 200);
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (items, total);
        }

        /// <inheritdoc />
        public async Task<int> UpdateUserAsync(User user, CancellationToken cancellationToken = default)
        {
            context.Users.Update(user);
            return await context.SaveChangesAsync();
        }
    }
}