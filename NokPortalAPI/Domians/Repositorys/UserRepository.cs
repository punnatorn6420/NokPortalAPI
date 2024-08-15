using System.Data;
using Dapper;
using Newtonsoft.Json;
using NokCore.Exceptions;
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

        public async Task<UserAppWithEnvRoles> GetUserAppsWithEnvRolesAsync(IDbConnection conn, IDbTransaction tran, int userId)
        {
            var userQuery = @"
        SELECT
            UserId,
            ObjectId,
            FirstName,
            LastName,
            Email,
            JobTitle,
            Department,
            Active,
            CreatedAt,
            ModifiedAt
        FROM Users
        WHERE UserId = @UserId;";

            var appsQuery = @"
        SELECT
            a.AppID,
            a.Name AS AppName,
            ae.Environment,
            ae.BaseURL,
            ae.Additional,
            ae.SecretKey,
            ae.JwtHourLimit
        FROM
            Assigned_Users au
            JOIN Apps a ON au.AppID = a.AppID
            JOIN Apps_Env ae ON a.AppID = ae.AppID AND au.Environment = ae.Environment
        WHERE
            au.UserID = @UserId;";

            var rolesQuery = @"
        SELECT
            au.AppID,
            au.Environment,
            ar.RoleID
        FROM
            Assigned_Users au
            LEFT JOIN Apps_Roles ar ON au.AssignedUserID = ar.AssignedUserID
        WHERE
            au.UserID = @UserId;";

            var commandText = $"{userQuery};{appsQuery};{rolesQuery}";

            using (var multi = await conn.QueryMultipleAsync(new CommandDefinition(
                commandText: commandText,
                parameters: new { UserId = userId },
                transaction: tran)))
            {
                // Map User
                var user = await multi.ReadSingleOrDefaultAsync<User>();

                // Map Applications and Environment Details
                var appEnvDetails = (await multi.ReadAsync<dynamic>()).ToList(); // Convert to List<dynamic>

                // Map Roles
                var roles = (await multi.ReadAsync<dynamic>()).ToList(); // Convert to List<dynamic>

                // Process applications and roles
                var appWithEnvRolesDict = new Dictionary<int, AppWithEnvRoles>();

                foreach (var appEnv in appEnvDetails)
                {
                    var appId = (int)appEnv.AppID;
                    if (!appWithEnvRolesDict.TryGetValue(appId, out var appWithEnvRoles))
                    {
                        appWithEnvRoles = new AppWithEnvRoles
                        {
                            AppId = appId,
                            Name = appEnv.AppName,
                            EnvironmentRoles = new List<EnvRoles>() // Initialize as List
                        };
                        appWithEnvRolesDict.Add(appId, appWithEnvRoles);
                    }

                    var envRole = new EnvRoles
                    {
                        Environment = (EnumEnvironmentType)Enum.Parse(typeof(EnumEnvironmentType), (string)appEnv.Environment),
                        Roles = roles.Where(r => r.AppID == appId && r.Environment == appEnv.Environment).Select(r => (int)r.RoleID).ToList()
                    };

                    ((List<EnvRoles>)appWithEnvRoles.EnvironmentRoles).Add(envRole);
                }

                return new UserAppWithEnvRoles
                {
                    User = user ?? throw new DataValidationException("user null value"),
                    AppWithEnvRoles = appWithEnvRolesDict.Values
                };
            }
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
                throw new DataValidationException("This email already exists in the system.");
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
