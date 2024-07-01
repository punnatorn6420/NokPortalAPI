using System.Data;
using NokCore.Identity.Models;
using NokPortal.Domians.Models;
using NokPortalAPI.Domians.Models;

namespace NokPortal.Domains.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllUsersAsync(IDbConnection conn, IDbTransaction tran);

        Task<IEnumerable<UserApps>> GetUserAppsAsync(IDbConnection conn, IDbTransaction tran, int userId);

        Task<User> GetUserByIdAsync(IDbConnection conn, IDbTransaction tran, int id);

        Task<int> CreateUserAsync(IDbConnection conn, IDbTransaction tran, User user);

        Task<bool> UpdateUserAsync(IDbConnection conn, IDbTransaction tran, User user);

        Task<bool> DeleteUserAsync(IDbConnection conn, IDbTransaction tran, int id);

        Task<User> GetUserByEmailAsync(IDbConnection conn, IDbTransaction tran, string email);
    }
}
