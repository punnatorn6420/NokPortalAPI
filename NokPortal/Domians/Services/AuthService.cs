using System.Data;
using NokPortal.Domains.Repositories;
using NokPortal.Domains.Services;
using NokPortal.Domians.Models;
using NokPortal.Shared.DB;

namespace NokPortal.Domians.Services
{
    public class AuthService : IAuthService
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly IConfiguration _configuration;
        private readonly IUsersRepository _userRepository;

        public AuthService(IDbConnectionFactory connectionFactory, IConfiguration configuration, IUsersRepository userRepository)
        {
            _connectionFactory = connectionFactory;
            _configuration = configuration;
            _userRepository = userRepository;
        }

        public async Task<Users> GetUsersByIdAsync(int userID)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            using var tran = connection.BeginTransaction();

            Users user = await _userRepository.GetUserByIdAsync(connection, tran, userID);
            tran.Commit();
            return user;
        }
    }
}
