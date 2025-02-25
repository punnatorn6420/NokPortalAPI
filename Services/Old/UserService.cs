namespace NokPortalAPI.Services.Old
{
    using NokCore.Api.JWT.Models;
    using NokCore.Api.JWT.Services;
    using NokCore.Identity.Models;
    using NokPortalAPI.Models;
    using NokPortalAPI.Repositories.Old;
    using NokPortalAPI.Shareds.DB;
    using System.Text.Json;

    /// <summary>
    /// This class implements the user service.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IConfiguration configuration;
        private readonly IUserRepository userRepository;
        private readonly IAppsRolesRepositorys appsRolesRepository;
        private readonly DbConnectionFactory dbConnectionFactory;
        private readonly HttpClient httpClient;
        private readonly IJwtService jwtService;

        public UserService(IConfiguration configuration, DbConnectionFactory dbConnectionFactory, IUserRepository userRepository, HttpClient httpClient, IJwtService jwtService, IAppsRolesRepositorys appsRolesRepository)
        {
            this.userRepository = userRepository;
            this.appsRolesRepository = appsRolesRepository;
            this.configuration = configuration;
            this.dbConnectionFactory = dbConnectionFactory;
            this.httpClient = httpClient;
            this.jwtService = jwtService;
        }

        /// <summary>
        /// Get all users from the database.
        /// </summary>
        /// <returns>The list of users.</returns>
        public async Task<IEnumerable<IUser>> GetAllUsersAsync()
        {
            using var conn = await dbConnectionFactory.CreateConnectionAsync();
            using var tran = conn.BeginTransaction();
            try
            {
                IEnumerable<IUser> listUser = await userRepository.GetAllUsersAsync(conn, tran);
                tran.Commit();
                return listUser;
            }
            catch
            {
                throw;
            }
        }

        public async Task<UserApps?> GetUserAppsAsync(int userId, EnumEnvironmentType env)
        {
            using var conn = await dbConnectionFactory.CreateConnectionAsync();
            using var tran = conn.BeginTransaction();
            try
            {
                IUser user = await userRepository.GetUserByIdAsync(conn, userId, tran);

                if (user == null)
                {
                    return null;
                }

                var appsWithRoles = await appsRolesRepository.GetAppsWithRolesByUserIdAsync(conn, userId, env, tran);

                var userApps = new UserApps
                {
                    User = user,
                    App = appsWithRoles.ToList()
                };

                tran.Commit();
                return userApps;
            }
            catch (Exception)
            {
                tran.Rollback();
                throw;
            }
        }

        public async Task<UserAppWithEnvRoles> GetUserAppsWithEnvRolesAsync(int userId)
        {
            using var conn = await dbConnectionFactory.CreateConnectionAsync();
            using var tran = conn.BeginTransaction();
            try
            {
                UserAppWithEnvRoles userApps = await userRepository.GetUserAppsWithEnvRolesAsync(conn, userId, tran);
                tran.Commit();
                return userApps;
            }
            catch
            {
                throw;
            }
        }

        public async Task<IUser> GetUserByIdAsync(int id)
        {
            using var conn = await dbConnectionFactory.CreateConnectionAsync();
            using var tran = conn.BeginTransaction();
            IUser user = await userRepository.GetUserByIdAsync(conn, id, tran);
            tran.Commit();
            return user;
        }

        public async Task<int> CreateUserAsync(IUser user)
        {
            using var conn = await dbConnectionFactory.CreateConnectionAsync();
            using var tran = conn.BeginTransaction();

            user.CreatedAt = DateTime.UtcNow;
            user.ModifiedAt = DateTime.UtcNow;
            int rowsAffected = await userRepository.CreateUserAsync(conn, user, tran);
            tran.Commit();
            return rowsAffected;
        }

        public async Task<bool> UpdateUserAsync(IUser user)
        {
            using var conn = await dbConnectionFactory.CreateConnectionAsync();
            using var tran = conn.BeginTransaction();

            user.ModifiedAt = DateTime.UtcNow;
            bool success = await userRepository.UpdateUserAsync(conn, user, tran);
            tran.Commit();
            return success;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            using var conn = await dbConnectionFactory.CreateConnectionAsync();
            using var tran = conn.BeginTransaction();

            bool success = await userRepository.DeleteUserAsync(conn, id, tran);
            tran.Commit();
            return success;
        }

        public string GenerateAuthorizationUrlSignin()
        {
            var clientId = configuration["OAuth2:ClientId"] ?? throw new ArgumentNullException("OAuth2:ClientId configuration is missing");
            var responseType = configuration["OAuth2:ResponseType"] ?? throw new ArgumentNullException("OAuth2:ResponseType configuration is missing");
            var redirectUri = configuration["OAuth2:RedirectUriSigIn"] ?? throw new ArgumentNullException("OAuth2:RedirectUriSigIn configuration is missing");
            var scope = configuration["OAuth2:Scope"] ?? throw new ArgumentNullException("OAuth2:Scope configuration is missing");
            var state = configuration["OAuth2:State"] ?? throw new ArgumentNullException("OAuth2:State configuration is missing");
            var tenantId = configuration["OAuth2:TenantId"] ?? throw new ArgumentNullException("OAuth2:TenantId configuration is missing");
            var authorizationEndpoint = $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/authorize";
            var authorizationUrl = $"{authorizationEndpoint}?client_id={Uri.EscapeDataString(clientId)}&response_type={Uri.EscapeDataString(responseType)}&redirect_uri={Uri.EscapeDataString(redirectUri)}&scope={Uri.EscapeDataString(scope)}&state={Uri.EscapeDataString(state)}";

            return authorizationUrl;
        }

        public async Task SignupMicrosoftGraphGetMeAsync(string token)
        {
            // Request to microsoft get AD info
            var request = new HttpRequestMessage(HttpMethod.Get, "https://graph.microsoft.com/v1.0/me");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await httpClient.SendAsync(request);
            ResponseMicrosoftUserInfo userAD;
            try
            {
                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync();
                userAD = JsonSerializer.Deserialize<ResponseMicrosoftUserInfo>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

                if (userAD == null)
                {
                    throw new InvalidOperationException("Deserialization returned null.");
                }
            }
            catch (Exception)
            {
                throw;
            }

            // Insert database create user
            User user = new User
            {
                FirstName = userAD.GivenName,
                LastName = userAD.Surname,
                Email = userAD.UserPrincipalName,
                JobTitle = userAD.JobTitle,
                ObjectId = userAD.Id,
                Department = userAD.OfficeLocation,
                CreatedAt = DateTime.Now,
                ModifiedAt = DateTime.Now,
                Active = true,
            };

            using var conn = await dbConnectionFactory.CreateConnectionAsync();
            using var tran = conn.BeginTransaction();

            int rowsAffected = await userRepository.CreateUserAsync(conn, user, tran);
            if (rowsAffected > 0)
            {
                tran.Commit();
            }
            else
            {
                throw new Exception("No recode create.");
            }
        }

        public async Task<JwtResponse> SigninMicrosoftGraphGetMeAsync(string token)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "https://graph.microsoft.com/v1.0/me");
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var response = await httpClient.SendAsync(request);
                ResponseMicrosoftUserInfo userAD;
                try
                {
                    response.EnsureSuccessStatusCode();
                    var responseContent = await response.Content.ReadAsStringAsync();

                    userAD = JsonSerializer.Deserialize<ResponseMicrosoftUserInfo>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
                    if (userAD == null)
                    {
                        throw new InvalidOperationException("Deserialization returned null.");
                    }
                }
                catch (Exception)
                {
                    throw;
                }

                using var conn = await dbConnectionFactory.CreateConnectionAsync();
                using var tran = conn.BeginTransaction();
                IUser user = await userRepository.GetUserByEmailAsync(conn, userAD.UserPrincipalName, tran);
                tran.Commit();

                var jwtData = new UserClaims
                {
                    UserId = user.Id,

                    // RoleId = (int)EnumUserRole.Root
                };

                return jwtService.GenerateToken(jwtData);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
