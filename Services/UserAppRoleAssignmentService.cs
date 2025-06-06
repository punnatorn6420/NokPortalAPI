using Newtonsoft.Json;
using NokAir.Core.Exceptions;
using NokAir.Shared.Api.Responses.Dtos.InHouse;
using NokAir.Shared.Security.Models.Common;
using NokAir.Shared.Security.Models.InHouse;
using NokAir.Shared.Security.Services.InHouse;
using NokPortalAPI.Dtos;
using NokPortalAPI.Entities;
using NokPortalAPI.Repositories;
using System.Security.Claims;
using System.Text;


namespace NokPortalAPI.Services
{
    /// <summary>
    /// Represents the service for user app role assignment.
    /// </summary>
    public class UserAppRoleAssignmentService : IUserAppRoleAssignmentService
    {
        private readonly AppDbContext context;
        private readonly IJwtService jwtService;
        private readonly IAppRepository appRepository;
        private readonly IUserAppRoleAssignmentRepository userAppRoleAssignmentRepository;
        private readonly IUserRepository<User> userRepository;
        private readonly HttpClient httpClient;

        public UserAppRoleAssignmentService(
            AppDbContext context,
            HttpClient httpClient,
            IAppRepository appRepository,
            IUserAppRoleAssignmentRepository userAppRoleAssignmentRepository,
            IUserRepository<User> userRepository,
            IJwtService jwtService)
        {
            this.context = context;
            var httpClientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };
            this.httpClient = new HttpClient(httpClientHandler);
            this.appRepository = appRepository;
            this.userAppRoleAssignmentRepository = userAppRoleAssignmentRepository;
            this.userRepository = userRepository;
            this.jwtService = jwtService;
        }

        /// <inheritdoc />
        public async Task<bool> AssignUserToAppAsync(UserAppAssignmentRequest userAppAssignmentReq)
        {
            var app = await appRepository.GetAppByIdAsync(userAppAssignmentReq.AppId)
                ?? throw new DataValidationException("Application not found. Please check the app.");

            var user = await userRepository.GetUserByIdAsync(userAppAssignmentReq.UserId)
                ?? throw new DataValidationException("User not found. Please check the user.");

            if (userAppAssignmentReq.Roles == null || !userAppAssignmentReq.Roles.Any())
                throw new DataValidationException("At least one role must be assigned.");

            var duplicatedRoles = new List<int>();
            foreach (var roleId in userAppAssignmentReq.Roles)
            {
                if (await userAppRoleAssignmentRepository.IsUserAppRoleExistAsync(userAppAssignmentReq.UserId, userAppAssignmentReq.AppId, roleId))
                {
                    duplicatedRoles.Add(roleId);
                }
            }
            if (duplicatedRoles.Any())
            {
                var rolesText = string.Join(", ", duplicatedRoles);
                throw new DataValidationException($"User is already assigned to these role(s) in the app: {rolesText}. Please re-check your selection.");
            }

            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                foreach (var roleId in userAppAssignmentReq.Roles)
                {
                    await userAppRoleAssignmentRepository.AddUserAppRoleAssignmentAsync(new UserAppRoleAssignment
                    {
                        UserId = userAppAssignmentReq.UserId,
                        AppId = userAppAssignmentReq.AppId,
                        RoleId = roleId
                    });
                }

                await context.SaveChangesAsync();

                var claims = new List<Claim>
                {
                    new("objectId", user.ObjectId),
                    new("firstName", user.FirstName),
                    new("lastName", user.LastName),
                    new("email", user.Email),
                    new("jobTitle", user.JobTitle),
                    new("department", user.Department),
                    new("active", user.Active.ToString().ToLowerInvariant()),
                    new("roles", JsonConvert.SerializeObject(userAppAssignmentReq.Roles))
                };
                var jwtSettings = new JwtSettingsModel
                {
                    SecretKey = app.SecretKey,
                    ExpiryInHours = app.JwtExpiryHours,
                    Issuer = string.Empty,
                    Audience = string.Empty
                };

                var jwtInfo = jwtService.GenerateJwtTokenInfo(claims.ToArray(), jwtSettings);

                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwtInfo.Token);

                var backendUrl = app.BackendUrl.TrimEnd('/');
                var content = new StringContent("{}", Encoding.UTF8, "application/json");
                try
                {
                    var response = await httpClient.PostAsync($"{backendUrl}/flight-irop/v1/flight-irop/assignUser", content);
                    if (!response.IsSuccessStatusCode)
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        throw new DataValidationException($"Failed to sync user with app: {errorContent}");
                    }
                }
                catch (Exception ex)
                {
                    throw new DataValidationException($"Failed to sync user with app: {ex.Message}");
                }
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }


        /// <inheritdoc />
        public async Task<JwtInfoModel> GetJwtTokenInfoByUserAppAsync(int userId, int appId)
        {
            // Verify if the user has been assigned to the app
            var isUserAssignedToApp = await userAppRoleAssignmentRepository.IsUserAssignedToAppAsync(userId, appId);
            if (!isUserAssignedToApp)
            {
                throw new DataValidationException("User is not assigned to the app. Please check the assignment.");
            }

            var user = await userRepository.GetUserByIdAsync(userId);

            var roleIds = await userAppRoleAssignmentRepository.GetUserRoleIdsForAppAsync(userId, appId);

            var app = await appRepository.GetAppByIdAsync(appId);

            if (app == null)
            {
                throw new DataValidationException("Application not found. Please check the app.");
            }

            if (user == null)
            {
                throw new DataValidationException("User not found. Please check the user.");
            }

            if (roleIds == null || !roleIds.Any())
            {
                throw new DataValidationException("User has no roles assigned in the app. Please check the assignment.");
            }

            var claims = new List<Claim>
            {
                new Claim("objectId", user.ObjectId),
                new Claim("firstName", user.FirstName),
                new Claim("lastName", user.LastName),
                new Claim("email", user.Email),
                new Claim("jobTitle", user.JobTitle),
                new Claim("department", user.Department),
                new Claim("active", user.Active.ToString()),
                new Claim("roles", JsonConvert.SerializeObject(roleIds))
            };

            var jwtSettings = new JwtSettingsModel
            {
                SecretKey = app.SecretKey,
                ExpiryInHours = app.JwtExpiryHours,
                Issuer = string.Empty,
                Audience = string.Empty
            };

            return jwtService.GenerateJwtTokenInfo(claims.ToArray(), jwtSettings);
        }

        /// <inheritdoc />
        public async Task<IList<RoleDto>> GetRolesByAppIdAsync(int appId)
        {
            try
            {
                var app = await appRepository.GetAppByIdAsync(appId);
                if (app == null)
                {
                    throw new DataValidationException("Application not found");
                }
                JwtInfoModel jwtToken = jwtService.GenerateJwtTokenInfo((UserClaimsModel?)null);
                string appUrl = app.BackendUrl += "/roles";
                var request = new HttpRequestMessage(HttpMethod.Get, appUrl);
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", $" {jwtToken.Token}");
                var response = await httpClient.SendAsync(request);
                string responseContent = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var res = JsonConvert.DeserializeObject<SuccessResponseDto<IList<RoleDto>>>(responseContent) ?? throw new DataValidationException("Empty roles data");
                    return res.Data ?? throw new DataValidationException("Empty roles data");
                }
                else
                {
                    throw new Exception($"Error: {responseContent}");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
