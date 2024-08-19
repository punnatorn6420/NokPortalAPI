using System.Data;
using NokCore.Identity.Models;
using NokPortalAPI.Domains.Models;

namespace NokPortalAPI.Domains.Repositorys
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllUsersAsync(IDbConnection conn, IDbTransaction? tran = null);

        Task<UserAppWithEnvRoles> GetUserAppsWithEnvRolesAsync(IDbConnection conn, int userId, IDbTransaction? tran = null);

        Task<User> GetUserByIdAsync(IDbConnection conn, int id, IDbTransaction? tran = null);

        Task<int> CreateUserAsync(IDbConnection conn, User user, IDbTransaction? tran = null);

        Task<bool> UpdateUserAsync(IDbConnection conn, User user, IDbTransaction? tran = null);

        Task<bool> DeleteUserAsync(IDbConnection conn, int id, IDbTransaction? tran = null);

        Task<User> GetUserByEmailAsync(IDbConnection conn, string email, IDbTransaction? tran = null);
    }
}
