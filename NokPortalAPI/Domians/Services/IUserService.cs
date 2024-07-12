using NokCore.Api.JwtToken.Models;
using NokCore.Identity.Models;
using NokPortalAPI.Domains.Models;

namespace NokPortalAPI.Domains.Services
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllUsersAsync();

        Task<IEnumerable<UserApps>> GetUserAppsAsync(int userId);

        Task<int> CreateUserAsync(User user);

        Task<bool> UpdateUserAsync(User user);

        Task<bool> DeleteUserAsync(int userId);

        string GenerateAuthorizationUrlSignup();

        string GenerateAuthorizationUrlSignin();

        Task SignupMicrosoftGraphGetMeAsync(string token);

        Task<ResponseJwt> SigninMicrosoftGraphGetMeAsync(string token);
    }
}
