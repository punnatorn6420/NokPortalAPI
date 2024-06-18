using System.Data;
using NokPortal.Domians.Models;

namespace NokPortal.Domains.Repositories
{
    public interface IUsersRepository
    {
        Task<IEnumerable<Users>> GetAllUsersAsync(IDbConnection conn, IDbTransaction tran);

        Task<Users> GetUserByIdAsync(IDbConnection conn, IDbTransaction tran, int id);

        Task<int> CreateUserAsync(IDbConnection conn, IDbTransaction tran, Users user);

        Task<bool> UpdateUserAsync(IDbConnection conn, IDbTransaction tran, Users user);

        Task<bool> DeleteUserAsync(IDbConnection conn, IDbTransaction tran, int id);

        Task<Users> GetUserByEmailAsync(IDbConnection conn, IDbTransaction tran, string email);
    }
}
