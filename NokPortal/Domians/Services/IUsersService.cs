using NokPortal.Domians.Models;

namespace NokPortal.Domains.Services
{
    public interface IUsersService
    {
        Task<IEnumerable<Users>> GetAllUsersAsync();

        Task<int> CreateUserAsync(Users user);

        Task<bool> UpdateUserAsync(Users user);

        Task<bool> DeleteUserAsync(int id);

        string GenerateAuthorizationUrlSignup();

        string GenerateAuthorizationUrlSignin();

        Task SignupMicrosoftGraphGetMeAsync(string token);

        Task<ResponseJwt> SigninMicrosoftGraphGetMeAsync(string token);
    }
}
