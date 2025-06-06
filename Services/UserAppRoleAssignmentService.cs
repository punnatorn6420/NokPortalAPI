using Newtonsoft.Json;
using NokAir.Core.Exceptions;
using NokAir.Shared.Api.Responses.Dtos.InHouse;
using NokAir.Shared.Security.Models.Common;
using NokAir.Shared.Security.Models.InHouse;
using NokAir.Shared.Security.Services.InHouse;
using NokPortalAPI.Dtos;
using NokPortalAPI.Entities;
using NokPortalAPI.Repositories;

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
            var app = await appRepository.GetAppByIdAsync(userAppAssignmentReq.AppId);
            if (app == null)
            {
                throw new DataValidationException("Application not found. Please check the app.");
            }

            var user = await userRepository.GetUserByIdAsync(userAppAssignmentReq.UserId);
            if (user == null)
            {
                throw new DataValidationException("User not found. Please check the user.");
            }

            if (userAppAssignmentReq.Roles == null || !userAppAssignmentReq.Roles.Any())
            {
                throw new DataValidationException("At least one role must be assigned.");
            }

            var duplicatedRoles = new List<int>();
            foreach (var roleId in userAppAssignmentReq.Roles)
            {
                var exists = await userAppRoleAssignmentRepository
                    .IsUserAppRoleExistAsync(userAppAssignmentReq.UserId, userAppAssignmentReq.AppId, roleId);

                if (exists)
                {
                    duplicatedRoles.Add(roleId);
                }
            }
            if (duplicatedRoles.Count != 0)
            {
                Console.WriteLine($"User {userAppAssignmentReq.UserId} is already assigned to these roles in the app {userAppAssignmentReq.AppId}: {string.Join(", ", duplicatedRoles)}");
                var rolesText = string.Join(", ", duplicatedRoles);
                throw new DataValidationException($"User is already assigned to these role(s) in the app: {rolesText}. Please re-check your selection.");
            }

            // Begin transaction
            using var transaction = context.Database.BeginTransaction();

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

            // Get app information
            var app = await appRepository.GetAppByIdAsync(appId);
            if (app == null)
            {
                throw new DataValidationException("Application not found. Please check the app.");
            }

            var jwtUserClaims = new UserClaimsModel()
            {
                UserId = userId
            };
            return jwtService.GenerateJwtTokenInfo(jwtUserClaims);
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
