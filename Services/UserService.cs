using NokCore.Api.JWT.Services;
using NokCore.Identity.Models;
using NokPortalAPI.Models;
using NokPortalAPI.Repositories;

namespace NokPortalAPI.Services
{
    /// <summary>
    /// User service.
    /// </summary>
    public class UserService : IUserService<User>
    {
        private readonly AppDbContext context;
        private readonly IUserRepository<User> userRepository;

        public UserService(
            AppDbContext context,
            IUserRepository<User> userRepository,
            IJwtService jwtService)
        {
            this.context = context;
            this.userRepository = userRepository;
        }


        /// <inheritdoc />
        public async Task<User> AddUserAsync(User user)
        {
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                await userRepository.AddUserAsync(user);
                await transaction.CommitAsync();
                return user;
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
        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await userRepository.GetUserByIdAsync(id);
        }

        /// <inheritdoc />
        public async Task<ICollection<User>> GetUsersByAppIdAsync(int appId)
        {
            return await userRepository.GetUsersByAppIdAsync(appId);
        }

        /// <inheritdoc />
        public async Task<ICollection<User>> GetUsersByCriteriaAsync(UserSearchCriteria searchCriteria)
        {
            return await userRepository.GetUsersByCriteriaAsync(searchCriteria);
        }

        /// <inheritdoc />
        public async Task<bool> UpdateUserAsync(User user)
        {
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
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
    }
}
