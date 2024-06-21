using NokCore.Identity.Models;
using NokPortal.Domians.Models;

namespace NokPortal.Domains.Services
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllUsersAsync();

        Task<int> CreateUserAsync(User user);

        Task<bool> UpdateUserAsync(User user);

        Task<bool> DeleteUserAsync(int id);

        string GenerateAuthorizationUrlSignup();

        string GenerateAuthorizationUrlSignin();

        Task SignupMicrosoftGraphGetMeAsync(string token);

        Task<ResponseJwt> SigninMicrosoftGraphGetMeAsync(string token);
    }
}
