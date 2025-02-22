using NokCore.Api.JWT.Models;
using NokCore.Identity.Models;
using NokPortalAPI.Models;

namespace NokPortalAPI.Services.Old
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllUsersAsync();

        Task<UserApps?> GetUserAppsAsync(int userId, EnumEnvironmentType env);

        Task<UserAppWithEnvRoles> GetUserAppsWithEnvRolesAsync(int userId);

        Task<int> CreateUserAsync(User user);

        Task<bool> UpdateUserAsync(User user);

        Task<bool> DeleteUserAsync(int userId);

        string GenerateAuthorizationUrlSignup();

        string GenerateAuthorizationUrlSignin();

        Task SignupMicrosoftGraphGetMeAsync(string token);

        Task<JwtResponse> SigninMicrosoftGraphGetMeAsync(string token);
    }
}
