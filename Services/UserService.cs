using Microsoft.EntityFrameworkCore;
using NokAir.Core.Abstractions.Services.Rbac;
using NokAir.Core.Exceptions;
using NokPortalAPI.Dtos;
using NokPortalAPI.Entities;
using NokPortalAPI.Extensions;
using NokPortalAPI.Repositories;
using System.Linq.Dynamic.Core;

namespace NokPortalAPI.Services
{
    /// <summary>
    /// User service.
    /// </summary>
    public class UserService : IUserService<UserDto>
    {
        private readonly AppDbContext context;
        private readonly IUserRepository<User> userRepository;
        private readonly IRoleRepository<Role> roleRepository;

        /// <inheritdoc/>
        public UserService(AppDbContext context, IUserRepository<User> userRepository, IRoleRepository<Role> roleRepository)
        {
            this.context = context;
            this.userRepository = userRepository;
            this.roleRepository = roleRepository;
        }

        /// <inheritdoc />
        public async Task<UserDto> AddUserAsync(UserDto userDto, CancellationToken cancellationToken = default)
        {
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var user = userDto.ToEntity();
                var createdUser = await userRepository.AddUserAsync(user);
                await roleRepository.AddUserRoleAsync(createdUser.Id, 3);
                await transaction.CommitAsync();
                return user.ToDto();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await userRepository.FindUserByEmailAsync(email);
        }

        /// <inheritdoc />
        public async Task<UserDto?> GetUserByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var user = await userRepository.FindUserByIdAsync(id);
            return user?.ToDto();
        }

        /// <inheritdoc/>
        public async Task<MyProfileDto?> GetMyProfileAsync(int userId)
        {
            return await userRepository.FindMyProfileAsync(userId);
        }

        /// <inheritdoc />
        public async Task<ICollection<UserDto>> GetUsersByAppIdAsync(int appId)
        {
            var users = await userRepository.FindUsersByAppIdAsync(appId);
            return users.Select(u => u.ToDto()).ToList();
        }

        /// <inheritdoc />
        public async Task<(IList<UserDto> Items, int TotalRecords)> GetUsersByCriteriaAsync(UserSearchCriteriaDto searchCriteriaDto)
        {
            var criteria = searchCriteriaDto.ToEntity();
            var (users, total) = await userRepository.FindUsersByCriteriaAsync(criteria);
            return (users.Select(u => u.ToDto()).ToList(), total);
        }

        /// <inheritdoc />
        public async Task<bool> UpdateUserAsync(UserDto userDto, CancellationToken cancellationToken = default)
        {
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var user = userDto.ToEntity();
                var rowsAffected = await userRepository.UpdateUserAsync(user);
                await transaction.CommitAsync();
                return rowsAffected > 0;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        async Task<UserDto?> IUserServiceBase<UserDto>.GetUserByEmailAsync(string email, CancellationToken cancellationToken)
        {
            var user = await userRepository.FindUserByEmailAsync(email);
            return user?.ToDto();
        }

        /// <inheritdoc/>
        public async Task UpdateUserRoleAsync(int userId, int roleId)
        {
            using var transaction = await context.Database.BeginTransactionAsync();

            var user = await userRepository.FindUserByIdAsync(userId);
            if (user == null)
            {
                throw new DataValidationException("User not found.");
            }

            var roleExists = await context.Roles.AnyAsync(r => r.Id == roleId);
            if (!roleExists)
            {
                throw new DataValidationException("Role does not exist.");
            }

            // Remove all existing roles assigned to this user
            // This is safe only if each user is supposed to have a single role
            var existingRoles = await context.UserRoles
            .Where(ur => ur.UserId == userId)
            .ToListAsync();

            if (existingRoles.Count != 0)
            {
                context.UserRoles.RemoveRange(existingRoles);
            }

            var newUserRole = new UserRole
            {
                UserId = userId,
                RoleId = roleId,
            };
            await context.UserRoles.AddAsync(newUserRole);

            await context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
    }
}