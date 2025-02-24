using System.Data;
using NokCore.Identity.Models;
using NokPortalAPI.Models;

namespace NokPortalAPI.Repositories.Old
{
    public interface IUserRepository
    {
        Task<IEnumerable<IUser>> GetAllUsersAsync(IDbConnection conn, IDbTransaction? tran = null);

        Task<UserAppWithEnvRoles> GetUserAppsWithEnvRolesAsync(IDbConnection conn, int userId, IDbTransaction? tran = null);

        Task<IUser> GetUserByIdAsync(IDbConnection conn, int id, IDbTransaction? tran = null);

        Task<int> CreateUserAsync(IDbConnection conn, IUser user, IDbTransaction? tran = null);

        Task<bool> UpdateUserAsync(IDbConnection conn, IUser user, IDbTransaction? tran = null);

        Task<bool> DeleteUserAsync(IDbConnection conn, int id, IDbTransaction? tran = null);

        Task<IUser> GetUserByEmailAsync(IDbConnection conn, string email, IDbTransaction? tran = null);
    }
}
