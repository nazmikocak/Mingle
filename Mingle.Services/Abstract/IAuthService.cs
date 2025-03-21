using Mingle.Shared.DTOs.Request;

namespace Mingle.Services.Abstract
{
    /// <summary>
    /// Kullanıcı kimlik doğrulama işlemleri için gerekli servis arayüzü.
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Yeni bir kullanıcı kaydı oluşturur.
        /// </summary>
        /// <param name="dto">Kullanıcı kaydı için gerekli bilgileri içeren DTO</param>
        /// <returns>Asenkron bir işlem döner.</returns>
        Task SignUpAsync(SignUp dto);



        /// <summary>
        /// E-posta ile kullanıcı girişini gerçekleştirir.
        /// </summary>
        /// <param name="dto">E-posta ve şifre bilgilerini içeren DTO</param>
        /// <returns>Kimlik doğrulama token'ını döner.</returns>
        Task<string> SignInEmailAsync(SignInEmail dto);



        /// <summary>
        /// Google üzerinden kullanıcı girişini gerçekleştirir.
        /// </summary>
        /// <param name="dto">Google giriş bilgilerini içeren DTO</param>
        /// <returns>Kimlik doğrulama token'ını döner.</returns>
        Task<string> SignInGoogleAsync(SignInProvider dto);



        /// <summary>
        /// Facebook üzerinden kullanıcı girişini gerçekleştirir.
        /// </summary>
        /// <param name="dto">Facebook giriş bilgilerini içeren DTO</param>
        /// <returns>Kimlik doğrulama token'ını döner.</returns>
        Task<string> SignInFacebookAsync(SignInProvider dto);



        /// <summary>
        /// Şifre sıfırlama talebi oluşturur.
        /// </summary>
        /// <param name="email">Şifre sıfırlama talebi için kullanılan e-posta adresi</param>
        /// <returns>Asenkron bir işlem döner.</returns>
        Task ResetPasswordAsync(string email);
    }
}