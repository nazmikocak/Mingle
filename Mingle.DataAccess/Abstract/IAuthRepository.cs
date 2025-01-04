using Firebase.Auth;

namespace Mingle.DataAccess.Abstract
{
    public interface IAuthRepository
    {
        Task<UserCredential> CreateUserAsync(string email, string password, string displayName);

        Task<UserCredential> SignInUserAsync(string email, string password);

        Task ChangePasswordAsync(UserCredential userCredential, string newPasswordAgain);

        Task ResetEmailPasswordAsync(string email);
    }
}
