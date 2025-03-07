using Firebase.Auth;

namespace Mingle.DataAccess.Abstract
{
    public interface IAuthRepository
    {
        Task<UserCredential> CreateUserAsync(string email, string password, string displayName);

        Task<UserCredential> SignInWithEmailAsync(string email, string password);

        //Task<UserCredential> SignInWithGoogleAsync(string accessToken);

        Task<UserCredential> SignInWithFacebookAsync(string accessToken);

        Task ChangePasswordAsync(UserCredential userCredential, string newPasswordAgain);

        Task ResetEmailPasswordAsync(string email);
    }
}