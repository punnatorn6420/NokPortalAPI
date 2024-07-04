using System.Data;
using NokCore.Identity.Models;
using NokPortal.Domains.Repositories;
using NokPortal.Domains.Services;
using NokPortal.Domians.Models;
using NokPortal.Shared.DB;

namespace NokPortal.Domians.Services
{
    public class AuthService : IAuthService
    {
        private readonly IDbConnectionFactory connectionFactory;
        private readonly IConfiguration configuration;
        private readonly IUserRepository userRepository;

        public AuthService(IDbConnectionFactory connectionFactory, IConfiguration configuration, IUserRepository userRepository)
        {
            this.connectionFactory = connectionFactory;
            this.configuration = configuration;
            this.userRepository = userRepository;
        }

        public async Task<User> GetUsersByIdAsync(int userID)
        {
            using var connection = connectionFactory.CreateConnection();
            connection.Open();
            using var tran = connection.BeginTransaction();

            User user = await userRepository.GetUserByIdAsync(connection, tran, userID);
            tran.Commit();
            return user;
        }
    }
}
