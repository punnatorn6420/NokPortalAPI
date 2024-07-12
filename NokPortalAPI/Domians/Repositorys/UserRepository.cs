using System.Data;
using Dapper;
using NokCore.Identity.Models;
using NokPortalAPI.Domains.Models;

namespace NokPortalAPI.Domains.Repositorys
{
    public class UserRepository : IUserRepository
    {
        public async Task<IEnumerable<User>> GetAllUsersAsync(IDbConnection conn, IDbTransaction tran)
        {
            string query = "SELECT * FROM Users";
            return await conn.QueryAsync<User>(query, transaction: tran);
        }

        public async Task<User> GetUserByIdAsync(IDbConnection conn, IDbTransaction tran, int id)
        {
            string query = "SELECT * FROM Users WHERE UserID = @Id";
            User? appUser = await conn.QuerySingleOrDefaultAsync<User>(query, new { Id = id }, tran);
            if (appUser == null)
            {
                throw new Exception("Not found user");
            }

            return appUser;
        }

        public async Task<IEnumerable<UserApps>> GetUserAppsAsync(IDbConnection conn, IDbTransaction tran, int userID)
        {
            // Join query to retrieve user and their apps
            string query = @"
                SELECT 
                    u.UserID, u.Email, u.FirstName, u.LastName, u.JobTitle, u.Department, 
                    u.ObjectId, u.CreatedAt, u.ModifiedAt, u.Active,
                    a.AppId, a.Name, a.Header, a.Subheader, a.Detail, a.Image
                FROM Users u
                INNER JOIN Assigned_Users ua ON u.UserID = ua.UserID
                INNER JOIN Apps a ON ua.AppId = a.AppId
                WHERE u.UserID = @UserID";

            // Dictionary to hold the results and group by user
            var userAppDictionary = new Dictionary<int, UserApps>();

            var result = await conn.QueryAsync<User, App, UserApps>(
                query,
                (user, app) =>
                {
                    if (!userAppDictionary.TryGetValue(user.UserId, out var userApps))
                    {
                        userApps = new UserApps
                        {
                            User = user,
                            App = new List<App>() // Initialize as List<ModelApp>
                        };
                        userAppDictionary.Add(user.UserId, userApps);
                    }

                    ((List<App>)userApps.App).Add(app); // Cast to List<ModelApp> to use Add method
                    return userApps;
                },
                new { UserID = userID },
                splitOn: "AppId",
                transaction: tran
            );

            // Convert List<ModelApp> to IEnumerable<ModelApp> before returning
            foreach (var userApps in userAppDictionary.Values)
            {
                userApps.App = userApps.App.ToList();
            }

            return userAppDictionary.Values;
        }

        public async Task<int> CreateUserAsync(IDbConnection conn, IDbTransaction tran, User user)
        {
            string checkQuery = @"
                SELECT COUNT(*)
                FROM Users
                WHERE Email = @Email";

            int count = await conn.ExecuteScalarAsync<int>(checkQuery, new { Email = user.Email }, tran);

            // If count > 0, email already exists; handle accordingly
            if (count > 0)
            {
                // Handle duplicate email error or return a message indicating the email is not unique
                throw new DuplicateNameException("This email already exists in the system.");
            }
            else
            {
                // Email is unique, proceed with insert
                string insertQuery = @"
                    INSERT INTO Users (Email, FirstName, LastName, JobTitle, Department, ObjectId, CreatedAt, ModifiedAt, Active)
                    VALUES (@Email, @FirstName, @LastName, @JobTitle, @Department, @ObjectId, @CreatedAt, @ModifiedAt, @Active);
                    SELECT CAST(SCOPE_IDENTITY() as int);";

                // Execute insert query and return the newly inserted user's ID
                return await conn.QuerySingleAsync<int>(insertQuery, user, tran);
            }
        }

        public async Task<bool> UpdateUserAsync(IDbConnection conn, IDbTransaction tran, User user)
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
