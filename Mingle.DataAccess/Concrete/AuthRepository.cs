using Firebase.Auth;
using Microsoft.Extensions.Options;
using Mingle.DataAccess.Abstract;
using Mingle.DataAccess.Configurations;

namespace Mingle.DataAccess.Concrete
{
    /// <summary>
    /// Firebase kimlik doğrulama (Auth) işlemlerini yöneten depo (repository) sınıfı.
    /// </summary>
    public sealed class AuthRepository : IAuthRepository
    {
        private readonly FirebaseAuthClient _authClient;



        /// <summary>
        /// <see cref="AuthRepository"/> sınıfını belirtilen Firebase yapılandırması ile başlatır.
        /// </summary>
        /// <param name="firebaseConfig">Firebase kimlik doğrulama istemcisini içeren yapılandırma.</param>
        public AuthRepository(FirebaseConfig firebaseConfig)
        {
            _authClient = firebaseConfig.AuthClient;
        }



        /// <summary>
        /// Belirtilen e-posta, parola ve görünen ad ile yeni bir kullanıcı oluşturur.
        /// </summary>
        /// <param name="email">Kullanıcının e-posta adresi.</param>
        /// <param name="password">Kullanıcının parolası.</param>
        /// <param name="displayName">Kullanıcının görünen adı.</param>
        /// <returns>Oluşturulan kullanıcının kimlik bilgileri.</returns>
        public async Task<UserCredential> CreateUserAsync(string email, string password, string displayName)
        {
            return await _authClient.CreateUserWithEmailAndPasswordAsync(email, password, displayName);
        }



        /// <summary>
        /// E-posta ve parola ile oturum açar.
        /// </summary>
        /// <param name="email">Kullanıcının e-posta adresi.</param>
        /// <param name="password">Kullanıcının parolası.</param>
        /// <returns>Oturum açan kullanıcının kimlik bilgileri.</returns>
        public async Task<UserCredential> SignInWithEmailAsync(string email, string password)
        {
            return await _authClient.SignInWithEmailAndPasswordAsync(email, password);
        }



        /// <summary>
        /// Kullanıcının parolasını değiştirir.
        /// </summary>
        /// <param name="userCredential">Kimlik doğrulama bilgilerini içeren kullanıcı.</param>
        /// <param name="newPasswordAgain">Yeni parola.</param>
        public async Task ChangePasswordAsync(UserCredential userCredential, string newPasswordAgain)
        {
            await userCredential.User.ChangePasswordAsync(newPasswordAgain);
        }



        /// <summary>
        /// Belirtilen e-posta adresine parola sıfırlama bağlantısı gönderir.
        /// </summary>
        /// <param name="email">Kullanıcının e-posta adresi.</param>
        public async Task ResetEmailPasswordAsync(string email)
        {
            await _authClient.ResetEmailPasswordAsync(email);
        }
    }
}