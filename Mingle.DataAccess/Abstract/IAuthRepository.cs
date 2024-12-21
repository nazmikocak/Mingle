using Firebase.Auth;

namespace Mingle.DataAccess.Abstract
{
    public interface IAuthRepository
    {
        Task<UserCredential> SignUpUserAsync(string email, string password, string displayName);

        Task<UserCredential> SignInUserAsync(string email, string password);

    }
}
