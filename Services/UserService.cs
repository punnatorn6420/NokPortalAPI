using NokAir.Core.Interfaces.Rbac.Services;
using NokPortalAPI.Dtos;
using NokPortalAPI.Entities;
using NokPortalAPI.Extensions;
using NokPortalAPI.Repositories;
using Microsoft.EntityFrameworkCore;
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

        public UserService(AppDbContext context, IUserRepository<User> userRepository)
        {
            this.context = context;
            this.userRepository = userRepository;
        }


        /// <inheritdoc />
        public async Task<UserDto> AddUserAsync(UserDto userDto)
        {
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var user = userDto.ToEntity();
                await userRepository.AddUserAsync(user);
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
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await userRepository.GetUserByEmailAsync(email);
        }

        /// <inheritdoc />
        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            var user = await userRepository.GetUserByIdAsync(id);
            return user?.ToDto();
        }

        public async Task<MyProfileDto?> GetMyProfileAsync(int userId)
        {
            return await userRepository.GetMyProfileAsync(userId);
        }

        /// <inheritdoc />
        public async Task<ICollection<UserDto>> GetUsersByAppIdAsync(int appId)
        {
            var users = await userRepository.GetUsersByAppIdAsync(appId);
            return users.Select(u => u.ToDto()).ToList();
        }

        /// <inheritdoc />
        public async Task<ICollection<UserDto>> GetUsersByCriteriaAsync(UserSearchCriteriaDto searchCriteriaDto)
        {
            var searchCriteria = searchCriteriaDto.ToEntity();
            var users = await userRepository.GetUsersByCriteriaAsync(searchCriteria);
            return users.Select(u => u.ToDto()).ToList();
        }

        /// <inheritdoc />
        public async Task<bool> UpdateUserAsync(UserDto userDto)
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

        async Task<UserDto?> IUserServiceBase<UserDto>.GetUserByEmailAsync(string email)
        {
            var user = await userRepository.GetUserByEmailAsync(email);
            return user?.ToDto();
        }
    }
}
