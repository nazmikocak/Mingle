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

        public (bool IsValid, string ErrorMessage) ValidateGoogle(SignInGoogle dto)
        {
            if (!dto.ApiKey.Equals(_apiKey))
            {
                return (false, "API anahtarı geçersiz.");
            }

            //if (!dto.ProviderId.Equals("google.com"))
            //{
            //    return (false, "Geçersiz sağlayıcı. Google hesabı gereklidir.");
            //}

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