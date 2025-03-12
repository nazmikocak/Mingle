using Firebase.Auth;

namespace Mingle.DataAccess.Abstract
{
    /// <summary>
    /// Kimlik doğrulama işlemlerini yöneten arayüz.
    /// </summary>
    public interface IAuthRepository
    {
        /// <summary>
        /// Belirtilen e-posta, parola ve görünen ad ile yeni bir kullanıcı oluşturur.
        /// </summary>
        /// <param name="email">Kullanıcının e-posta adresi.</param>
        /// <param name="password">Kullanıcının parolası.</param>
        /// <param name="displayName">Kullanıcının görünen adı.</param>
        /// <returns>Oluşturulan kullanıcının kimlik bilgileri.</returns>
        Task<UserCredential> CreateUserAsync(string email, string password, string displayName);



        /// <summary>
        /// E-posta ve parola ile oturum açar.
        /// </summary>
        /// <param name="email">Kullanıcının e-posta adresi.</param>
        /// <param name="password">Kullanıcının parolası.</param>
        /// <returns>Oturum açan kullanıcının kimlik bilgileri.</returns>
        Task<UserCredential> SignInWithEmailAsync(string email, string password);



        /// <summary>
        /// Kullanıcının parolasını değiştirir.
        /// </summary>
        /// <param name="userCredential">Kimlik doğrulama bilgilerini içeren kullanıcı.</param>
        /// <param name="newPasswordAgain">Yeni parola.</param>
        Task ChangePasswordAsync(UserCredential userCredential, string newPasswordAgain);



        /// <summary>
        /// Belirtilen e-posta adresine parola sıfırlama bağlantısı gönderir.
        /// </summary>
        /// <param name="email">Kullanıcının e-posta adresi.</param>
        Task ResetEmailPasswordAsync(string email);
    }
}