using System.Data;
using System.Text.Json;
using NokCore.Api.JwtToken.Models;
using NokCore.Api.JwtToken.Services;
using NokCore.Identity.Models;
using NokPortalAPI.Domains.Models;
using NokPortalAPI.Domains.Repositorys;
using NokPortalAPI.Shared.DB;

namespace NokPortalAPI.Domains.Services
{
    public class UserService : IUserService
    {
        private readonly IConfiguration configuration;
        private readonly IUserRepository userRepository;
        private readonly IDbConnection connectionFactory;
        private readonly HttpClient httpClient;
        private readonly IJwtService jwtService;

        public UserService(IConfiguration configuration, IDbConnectionFactory connectionFactory, IUserRepository userRepository, HttpClient httpClient, IJwtService jwtService)
        {
            this.userRepository = userRepository;
            this.configuration = configuration;
            this.connectionFactory = connectionFactory.CreateConnection();
            this.httpClient = httpClient;
            this.jwtService = jwtService;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            connectionFactory.Open();
            using var tran = connectionFactory.BeginTransaction();
            try
            {
                IEnumerable<User> listUser = await userRepository.GetAllUsersAsync(connectionFactory, tran);
                tran.Commit();
                return listUser;
            }
            catch
            {
                throw;
            }
        }

        public async Task<IEnumerable<UserApps>> GetUserAppsAsync(int userId)
        {
            connectionFactory.Open();
            using var tran = connectionFactory.BeginTransaction();
            try
            {
                IEnumerable<UserApps> userApps = await userRepository.GetUserAppsAsync(connectionFactory, tran, userId);
                tran.Commit();
                return userApps;
            }
            catch
            {
                throw;
            }
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            connectionFactory.Open();
            using var tran = connectionFactory.BeginTransaction();
            User user = await userRepository.GetUserByIdAsync(connectionFactory, tran, id);
            tran.Commit();
            return user;
        }

        public async Task<int> CreateUserAsync(User user)
        {
            connectionFactory.Open();
            using var tran = connectionFactory.BeginTransaction();

            user.CreatedAt = DateTime.UtcNow;
            user.ModifiedAt = DateTime.UtcNow;
            int rowsAffected = await userRepository.CreateUserAsync(connectionFactory, tran, user);
            tran.Commit();
            return rowsAffected;
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            connectionFactory.Open();
            using var tran = connectionFactory.BeginTransaction();

            user.ModifiedAt = DateTime.UtcNow;
            bool success = await userRepository.UpdateUserAsync(connectionFactory, tran, user);
            tran.Commit();
            return success;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            connectionFactory.Open();
            using var tran = connectionFactory.BeginTransaction();

            bool success = await userRepository.DeleteUserAsync(connectionFactory, tran, id);
            tran.Commit();
            return success;
        }

        public string GenerateAuthorizationUrlSignup()
        {
            var clientId = configuration["OAuth2:ClientId"] ?? throw new ArgumentNullException("OAuth2:ClientId configuration is missing");
            var responseType = configuration["OAuth2:ResponseType"] ?? throw new ArgumentNullException("OAuth2:ResponseType configuration is missing");
            var redirectUri = configuration["OAuth2:RedirectUriSigUp"] ?? throw new ArgumentNullException("OAuth2:RedirectUriSigUp configuration is missing");
            var scope = configuration["OAuth2:Scope"] ?? throw new ArgumentNullException("OAuth2:Scope configuration is missing");
            var state = configuration["OAuth2:State"] ?? throw new ArgumentNullException("OAuth2:State configuration is missing");
            var tenantId = configuration["OAuth2:TenantId"] ?? throw new ArgumentNullException("OAuth2:TenantId configuration is missing");
            var authorizationEndpoint = $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/authorize";
            var authorizationUrl = $"{authorizationEndpoint}?client_id={Uri.EscapeDataString(clientId)}&response_type={Uri.EscapeDataString(responseType)}&redirect_uri={Uri.EscapeDataString(redirectUri)}&scope={Uri.EscapeDataString(scope)}&state={Uri.EscapeDataString(state)}";

            return authorizationUrl;
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

                userAD = JsonSerializer.Deserialize<ResponseMicrosoftUserInfo>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) !;

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

            connectionFactory.Open();
            using var tran = connectionFactory.BeginTransaction();

            int rowsAffected = await userRepository.CreateUserAsync(connectionFactory, tran, user);
            if (rowsAffected > 0)
            {
                tran.Commit();
            }
            else
            {
                throw new Exception("No recode create.");
            }
        }

        public async Task<ResponseJwt> SigninMicrosoftGraphGetMeAsync(string token)
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

                    userAD = JsonSerializer.Deserialize<ResponseMicrosoftUserInfo>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) !;
                    if (userAD == null)
                    {
                        throw new InvalidOperationException("Deserialization returned null.");
                    }
                }
                catch (Exception)
                {
                    throw;
                }

                connectionFactory.Open();
                using var tran = connectionFactory.BeginTransaction();
                User user = await userRepository.GetUserByEmailAsync(connectionFactory, tran, userAD.UserPrincipalName);
                tran.Commit();

                var jwtData = new JwtData
                {
                    UserId = user.UserId,
                    RoleId = (int)EnumUserRole.Root
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
