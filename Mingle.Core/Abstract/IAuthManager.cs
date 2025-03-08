using Mingle.Shared.DTOs.Request;

namespace Mingle.Core.Abstract
{
    /// <summary>
    /// Kullanıcı doğrulama işlemleri için bir sözleşme sağlayan arayüz.
    /// Bu arayüz, farklı sağlayıcılara yönelik oturum açma doğrulama işlemleri sağlar.
    /// </summary>
    public interface IAuthManager
    {
        /// <summary>
        /// Google sağlayıcısı üzerinden oturum açma işlemini doğrular.
        /// </summary>
        /// <param name="dto">Sağlayıcı bilgilerini içeren <see cref="SignInProvider"/> nesnesi.</param>
        /// <returns>Doğrulamanın sonucunu ve hata mesajını içeren bir tuple döner. IsValid = false ise hata mesajı içerir.</returns>
        (bool IsValid, string ErrorMessage) ValidateGoogleProvider(SignInProvider dto);



        /// <summary>
        /// Facebook sağlayıcısı üzerinden oturum açma işlemini doğrular.
        /// </summary>
        /// <param name="dto">Sağlayıcı bilgilerini içeren <see cref="SignInProvider"/> nesnesi.</param>
        /// <returns>Doğrulamanın sonucunu ve hata mesajını içeren bir tuple döner. IsValid = false ise hata mesajı içerir.</returns>
        (bool IsValid, string ErrorMessage) ValidateFacebookProvider(SignInProvider dto);
    }
}