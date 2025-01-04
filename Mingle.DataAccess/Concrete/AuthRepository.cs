using Firebase.Auth;
using Firebase.Auth.Providers;
using Mingle.DataAccess.Abstract;
using Mingle.DataAccess.Configurations;

namespace Mingle.DataAccess.Concrete
{
    public sealed class AuthRepository : IAuthRepository
    {
        private readonly FirebaseAuthClient _authClient;

        public AuthRepository(FirebaseConfig firebaseConfig)
        {
            _authClient = firebaseConfig.AuthClient;
        }


        public async Task<UserCredential> CreateUserAsync(string email, string password, string displayName)
        {
            return await _authClient.CreateUserWithEmailAndPasswordAsync(email, password, displayName);
        }


        public async Task<UserCredential> SignInUserAsync(string email, string password)
        {
            return await _authClient.SignInWithEmailAndPasswordAsync(email, password);
        }


        public async Task ChangePasswordAsync(UserCredential userCredential, string newPasswordAgain)
        {
            await userCredential.User.ChangePasswordAsync(newPasswordAgain);
        }


        public async Task ResetEmailPasswordAsync(string email)
        {
            await _authClient.ResetEmailPasswordAsync(email);
        }
    }
}