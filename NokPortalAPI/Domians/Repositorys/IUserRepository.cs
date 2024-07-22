using System.Data;
using NokCore.Identity.Models;
using NokPortalAPI.Domains.Models;

namespace NokPortalAPI.Domains.Repositorys
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllUsersAsync(IDbConnection conn, IDbTransaction tran);

        Task<IEnumerable<UserApps>> GetUserAppsAsync(IDbConnection conn, IDbTransaction tran, int userId, EnumEnvironmentType environment);

        Task<UserAppWithEnvRoles> GetUserAppsWithEnvRolesAsync(IDbConnection conn, IDbTransaction tran, int userId);

        Task<User> GetUserByIdAsync(IDbConnection conn, IDbTransaction tran, int id);

        Task<int> CreateUserAsync(IDbConnection conn, IDbTransaction tran, User user);

        Task<bool> UpdateUserAsync(IDbConnection conn, IDbTransaction tran, User user);

        Task<bool> DeleteUserAsync(IDbConnection conn, IDbTransaction tran, int id);

        Task<User> GetUserByEmailAsync(IDbConnection conn, IDbTransaction tran, string email);
    }
}
