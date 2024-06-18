using System.Data;
using System.Text.Json;
using NokPortal.Domains.Repositories;
using NokPortal.Domians.Models;
using NokPortal.Domians.Services;
using NokPortal.Shared.DB;

namespace NokPortal.Domains.Services
{
    public class UsersService : IUsersService
    {
        private readonly IConfiguration _configuration;
        private readonly IUsersRepository _appUserRepository;
        private readonly IDbConnection _connectionFactory;
        private readonly HttpClient _httpClient;
        private readonly IJwtService _jwtService;

        public UsersService(IConfiguration configuration, IDbConnectionFactory connectionFactory, IUsersRepository appUserRepository, HttpClient httpClient, IJwtService jwtService)
        {
            _appUserRepository = appUserRepository;
            _configuration = configuration;
            _connectionFactory = connectionFactory.CreateConnection();
            _httpClient = httpClient;
            _jwtService = jwtService;
        }

        public async Task<IEnumerable<Users>> GetAllUsersAsync()
        {
            _connectionFactory.Open();
            using var tran = _connectionFactory.BeginTransaction();
            IEnumerable<Users> listUser = await _appUserRepository.GetAllUsersAsync(_connectionFactory, tran);
            tran.Commit();
            return listUser;
        }

        public async Task<Users> GetUserByIdAsync(int id)
        {
            _connectionFactory.Open();
            using var tran = _connectionFactory.BeginTransaction();
            Users user = await _appUserRepository.GetUserByIdAsync(_connectionFactory, tran, id);
            tran.Commit();
            return user;
        }

        public async Task<int> CreateUserAsync(Users user)
        {
            _connectionFactory.Open();
            using var tran = _connectionFactory.BeginTransaction();

            user.CreatedAt = DateTime.UtcNow;
            user.ModifiedAt = DateTime.UtcNow;
            int rowsAffected = await _appUserRepository.CreateUserAsync(_connectionFactory, tran, user);
            tran.Commit();
            return rowsAffected;
        }

        public async Task<bool> UpdateUserAsync(Users user)
        {
            _connectionFactory.Open();
            using var tran = _connectionFactory.BeginTransaction();

            user.ModifiedAt = DateTime.UtcNow;
            bool success = await _appUserRepository.UpdateUserAsync(_connectionFactory, tran, user);
            tran.Commit();
            return success;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            _connectionFactory.Open();
            using var tran = _connectionFactory.BeginTransaction();

            bool success = await _appUserRepository.DeleteUserAsync(_connectionFactory, tran, id);
            tran.Commit();
            return success;
        }

        public string GenerateAuthorizationUrlSignup()
        {
            var clientId = _configuration["OAuth2:ClientId"] ?? throw new ArgumentNullException("OAuth2:ClientId configuration is missing");
            var responseType = _configuration["OAuth2:ResponseType"] ?? throw new ArgumentNullException("OAuth2:ResponseType configuration is missing");
            var redirectUri = _configuration["OAuth2:RedirectUriSigUp"] ?? throw new ArgumentNullException("OAuth2:RedirectUriSigUp configuration is missing");
            var scope = _configuration["OAuth2:Scope"] ?? throw new ArgumentNullException("OAuth2:Scope configuration is missing");
            var state = _configuration["OAuth2:State"] ?? throw new ArgumentNullException("OAuth2:State configuration is missing");
            var tenantId = _configuration["OAuth2:TenantId"] ?? throw new ArgumentNullException("OAuth2:TenantId configuration is missing");
            var authorizationEndpoint = $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/authorize";
            var authorizationUrl = $"{authorizationEndpoint}?client_id={Uri.EscapeDataString(clientId)}&response_type={Uri.EscapeDataString(responseType)}&redirect_uri={Uri.EscapeDataString(redirectUri)}&scope={Uri.EscapeDataString(scope)}&state={Uri.EscapeDataString(state)}";

            return authorizationUrl;
        }

        public string GenerateAuthorizationUrlSignin()
        {
            var clientId = _configuration["OAuth2:ClientId"] ?? throw new ArgumentNullException("OAuth2:ClientId configuration is missing");
            var responseType = _configuration["OAuth2:ResponseType"] ?? throw new ArgumentNullException("OAuth2:ResponseType configuration is missing");
            var redirectUri = _configuration["OAuth2:RedirectUriSigIn"] ?? throw new ArgumentNullException("OAuth2:RedirectUriSigIn configuration is missing");
            var scope = _configuration["OAuth2:Scope"] ?? throw new ArgumentNullException("OAuth2:Scope configuration is missing");
            var state = _configuration["OAuth2:State"] ?? throw new ArgumentNullException("OAuth2:State configuration is missing");
            var tenantId = _configuration["OAuth2:TenantId"] ?? throw new ArgumentNullException("OAuth2:TenantId configuration is missing");
            var authorizationEndpoint = $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/authorize";
            var authorizationUrl = $"{authorizationEndpoint}?client_id={Uri.EscapeDataString(clientId)}&response_type={Uri.EscapeDataString(responseType)}&redirect_uri={Uri.EscapeDataString(redirectUri)}&scope={Uri.EscapeDataString(scope)}&state={Uri.EscapeDataString(state)}";

            return authorizationUrl;
        }

        public async Task SignupMicrosoftGraphGetMeAsync(string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://graph.microsoft.com/v1.0/me");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);
            UserADresponse userAD;
            try
            {
                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync();

                userAD = JsonSerializer.Deserialize<UserADresponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) !;

                if (userAD == null)
                {
                    throw new InvalidOperationException("Deserialization returned null.");
                }
            }
            catch (Exception)
            {
                throw;
            }

            Users user = new Users
            {
                FirstName = userAD.GivenName,
                LastName = userAD.Surname,
                Email = userAD.UserPrincipalName,
                JobTitle = userAD.JobTitle,
                Department = userAD.OfficeLocation,
                CreatedAt = DateTime.Now,
                ModifiedAt = DateTime.Now,
                Active = true,
            };

            try
            {
                _connectionFactory.Open();
                using var tran = _connectionFactory.BeginTransaction();

                int rowsAffected = await _appUserRepository.CreateUserAsync(_connectionFactory, tran, user);
                if (rowsAffected > 0)
                {
                    tran.Commit();
                }
                else
                {
                    throw new Exception("No recode create.");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResponseJwt> SigninMicrosoftGraphGetMeAsync(string token)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "https://graph.microsoft.com/v1.0/me");
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.SendAsync(request);
                UserADresponse userAD;
                try
                {
                    response.EnsureSuccessStatusCode();
                    var responseContent = await response.Content.ReadAsStringAsync();

                    userAD = JsonSerializer.Deserialize<UserADresponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) !;
                    if (userAD == null)
                    {
                        throw new InvalidOperationException("Deserialization returned null.");
                    }
                }
                catch (Exception)
                {
                    throw;
                }

                _connectionFactory.Open();
                using var tran = _connectionFactory.BeginTransaction();
                Users user = await _appUserRepository.GetUserByEmailAsync(_connectionFactory, tran, userAD.UserPrincipalName);
                tran.Commit();

                var jwtSetting = new JWTsetting
                {
                    UserId = user.UserID,
                    ApplicationId = null
                };

                return _jwtService.GenerateToken(jwtSetting);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
