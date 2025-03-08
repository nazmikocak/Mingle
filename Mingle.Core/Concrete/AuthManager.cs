using Microsoft.Extensions.Configuration;
using Mingle.Core.Abstract;
using Mingle.Shared.DTOs.Request;

namespace Mingle.Core.Concrete
{
    public class AuthManager : IAuthManager
    {
        private readonly string _apiKey;



        public AuthManager(IConfiguration configuration)
        {
            _apiKey = configuration["FirebaseSettings:ApiKey"]!;
        }



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