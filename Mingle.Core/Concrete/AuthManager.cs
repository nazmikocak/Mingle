using Microsoft.Extensions.Configuration;
using Mingle.Core.Abstract;
using Mingle.Shared.DTOs.Request;

namespace Mingle.Core.Concrete
{
    /// <summary>
    /// Kullanıcı doğrulama işlemleri için gerekli olan sınıf.
    /// Bu sınıf, Google ve Facebook gibi sağlayıcılar üzerinden yapılan oturum açma işlemlerini doğrular.
    /// </summary>
    /// <remarks>
    /// Bu sınıf, Firebase API anahtarını doğrulamak ve sağlayıcıların oturum sürelerini kontrol etmek için kullanılır.
    /// </remarks>
    public class AuthManager : IAuthManager
    {
        private readonly string _apiKey;



        /// <summary>
        /// <see cref="AuthManager"/> sınıfının yapıcı metodudur.
        /// Firebase API anahtarını yapılandırma dosyasından alır.
        /// </summary>
        /// <param name="configuration">Yapılandırma ayarlarını içeren <see cref="IConfiguration"/> nesnesi.</param>
        public AuthManager(IConfiguration configuration)
        {
            _apiKey = configuration["FirebaseSettings:ApiKey"]!;
        }



        /// <summary>
        /// Google sağlayıcısı üzerinden oturum açma işlemini doğrular.
        /// </summary>
        /// <param name="dto">Sağlayıcı bilgilerini içeren <see cref="SignInProvider"/> nesnesi.</param>
        /// <returns>Doğrulamanın sonucunu ve hata mesajını içeren bir tuple döner. IsValid = false ise hata mesajı içerir.</returns>
        /// <remarks>
        /// Bu metod, Google sağlayıcı verilerini kontrol eder ve geçerli olup olmadığını doğrular.
        /// Ayrıca API anahtarının geçerliliğini ve oturum süresinin geçip geçmediğini de kontrol eder.
        /// </remarks>
        public (bool IsValid, string ErrorMessage) ValidateGoogleProvider(SignInProvider dto)
        {
            if (!dto.ProviderData[0].ProviderId.Equals("google.com"))
            {
                return (false, "Geçersiz sağlayıcı. Google hesabı gereklidir.");
            }

            if (!dto.ApiKey.Equals(_apiKey))
            {
                return (false, "API anahtarı geçersiz.");
            }

            var expirationTime = dto.StsTokenManager.ExpirationTime;
            var currentTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            if (expirationTime < currentTime)
            {
                return (false, "Oturum süresi dolmuş.");
            }

            return (true, "");
        }



        /// <summary>
        /// Facebook sağlayıcısı üzerinden oturum açma işlemini doğrular.
        /// </summary>
        /// <param name="dto">Sağlayıcı bilgilerini içeren <see cref="SignInProvider"/> nesnesi.</param>
        /// <returns>Doğrulamanın sonucunu ve hata mesajını içeren bir tuple döner. IsValid = false ise hata mesajı içerir.</returns>
        /// <remarks>
        /// Bu metod, Facebook sağlayıcı verilerini kontrol eder ve geçerli olup olmadığını doğrular.
        /// Ayrıca API anahtarının geçerliliğini ve oturum süresinin geçip geçmediğini de kontrol eder.
        /// </remarks>
        public (bool IsValid, string ErrorMessage) ValidateFacebookProvider(SignInProvider dto)
        {
            if (!dto.ProviderData[0].ProviderId.Equals("facebook.com"))
            {
                return (false, "Geçersiz sağlayıcı. Facebook hesabı gereklidir.");
            }

            if (!dto.ApiKey.Equals(_apiKey))
            {
                return (false, "API anahtarı geçersiz.");
            }

            var expirationTime = dto.StsTokenManager.ExpirationTime;
            var currentTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            if (expirationTime < currentTime)
            {
                return (false, "Oturum süresi dolmuş.");
            }

            return (true, "");
        }
    }
}