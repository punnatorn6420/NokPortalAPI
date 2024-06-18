using System.Data;
using Dapper;
using NokPortal.Domians.Models;

namespace NokPortal.Domains.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        public async Task<IEnumerable<Users>> GetAllUsersAsync(IDbConnection conn, IDbTransaction tran)
        {
            string query = "SELECT * FROM Users";
            return await conn.QueryAsync<Users>(query, tran);
        }

        public async Task<Users> GetUserByIdAsync(IDbConnection conn, IDbTransaction tran, int id)
        {
            string query = "SELECT * FROM Users WHERE UserID = @Id";
            Users? appUser = await conn.QuerySingleOrDefaultAsync<Users>(query, new { Id = id }, tran);
            if (appUser == null)
            {
                throw new Exception("Not found user");
            }

            return appUser;
        }

        public async Task<int> CreateUserAsync(IDbConnection conn, IDbTransaction tran, Users user)
        {
            string query = @"
                INSERT INTO Users (Email, FirstName, LastName, JobTitle, Department, CreatedAt, ModifiedAt, Active)
                VALUES (@Email, @FirstName, @LastName, @JobTitle, @Department, @CreatedAt, @ModifiedAt, @Active);
                SELECT CAST(SCOPE_IDENTITY() as int);";
            return await conn.QuerySingleAsync<int>(query, user, tran);
        }

        public async Task<bool> UpdateUserAsync(IDbConnection conn, IDbTransaction tran, Users user)
        {
            string query = @"
                UPDATE Users
                SET Email = @Email, FirstName = @FirstName, LastName = @LastName, JobTitle = @JobTitle,
                    Department = @Department, ModifiedAt = @ModifiedAt, Active = @Active
                WHERE UserID = @UserID;";
            int rowsAffected = await conn.ExecuteAsync(query, user, tran);
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteUserAsync(IDbConnection conn, IDbTransaction tran, int id)
        {
            string query = "DELETE FROM Users WHERE UserID = @Id";
            int rowsAffected = await conn.ExecuteAsync(query, id, tran);
            return rowsAffected > 0;
        }

        public async Task<Users> GetUserByEmailAsync(IDbConnection conn, IDbTransaction tran, string email)
        {
            string query = "SELECT * FROM Users WHERE Email = @Email";
            var queryByEmail = new { Email = email };
            Users? appUser = await conn.QuerySingleOrDefaultAsync<Users>(query, queryByEmail, tran);
            if (appUser == null)
            {
                throw new Exception("Not found user");
            }

            return appUser;
        }
    }
}
