using System.Data;
using Dapper;
using NokCore.Identity.Models;
using NokPortal.Domians.Models;

namespace NokPortal.Domains.Repositories
{
    public class UserRepository : IUserRepository
    {
        public async Task<IEnumerable<User>> GetAllUsersAsync(IDbConnection conn, IDbTransaction tran)
        {
            string query = "SELECT * FROM Users";
            return await conn.QueryAsync<User>(query, tran);
        }

        public async Task<User> GetUserByIdAsync(IDbConnection conn, IDbTransaction tran, int id)
        {
            string query = "SELECT * FROM Users WHERE UserId = @Id";
            User? appUser = await conn.QuerySingleOrDefaultAsync<User>(query, new { Id = id }, tran);
            if (appUser == null)
            {
                throw new Exception("Not found user");
            }

            return appUser;
        }

        public async Task<int> CreateUserAsync(IDbConnection conn, IDbTransaction tran, User user)
        {
            string query = @"
                INSERT INTO Users (Email, FirstName, LastName, JobTitle, Department, ObjectId, CreatedAt, ModifiedAt, Active)
                VALUES (@Email, @FirstName, @LastName, @JobTitle, @Department, @ObjectId, @CreatedAt, @ModifiedAt, @Active);
                SELECT CAST(SCOPE_IDENTITY() as int);";
            return await conn.QuerySingleAsync<int>(query, user, tran);
        }

        public async Task<bool> UpdateUserAsync(IDbConnection conn, IDbTransaction tran, User user)
        {
            string query = @"
                UPDATE Users
                SET Email = @Email, FirstName = @FirstName, LastName = @LastName, JobTitle = @JobTitle,
                    Department = @Department, ModifiedAt = @ModifiedAt, Active = @Active
                WHERE UserId = @UserId;";
            int rowsAffected = await conn.ExecuteAsync(query, user, tran);
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteUserAsync(IDbConnection conn, IDbTransaction tran, int id)
        {
            string query = "DELETE FROM Users WHERE UserId = @Id";
            int rowsAffected = await conn.ExecuteAsync(query, id, tran);
            return rowsAffected > 0;
        }

        public async Task<User> GetUserByEmailAsync(IDbConnection conn, IDbTransaction tran, string email)
        {
            string query = "SELECT * FROM Users WHERE Email = @Email";
            var queryByEmail = new { Email = email };
            User? appUser = await conn.QuerySingleOrDefaultAsync<User>(query, queryByEmail, tran);
            if (appUser == null)
            {
                throw new Exception("Not found user");
            }

            return appUser;
        }
    }
}
