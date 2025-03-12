using Firebase.Auth;
using Firebase.Auth.Providers;
using Firebase.Database;
using Microsoft.Extensions.Configuration;

namespace Mingle.DataAccess.Configurations
{
    /// <summary>
    /// Firebase kimlik doğrulama ve veritabanı erişim yapılandırmasını yönetir.
    /// </summary>
    public class FirebaseConfig
    {
        /// <summary> Firebase kimlik doğrulama işlemleri için kullanılan istemci nesnesidir.</summary>
        public FirebaseAuthClient AuthClient { get; }



        /// <summary> Firebase Realtime Database ile iletişim kurmak için kullanılan istemci nesnesidir.</summary>
        public FirebaseClient DatabaseClient { get; }



        // <summary>
        /// Firebase kimlik doğrulama ve veritabanı istemcilerini belirtilen <see cref="IConfiguration"/> nesnesine göre yapılandırır.
        /// </summary>
        /// <param name="configuration">Uygulamanın yapılandırma ayarlarını içeren <see cref="IConfiguration"/> nesnesi.</param>
        public FirebaseConfig(IConfiguration configuration)
        {
            var apiKey = configuration["FirebaseSettings:ApiKey"];
            var authDomain = configuration["FirebaseSettings:AuthDomain"];
            var databaseUrl = configuration["FirebaseSettings:DatabaseUrl"];

            var config = new FirebaseAuthConfig
            {
                ApiKey = apiKey,
                AuthDomain = authDomain,
                Providers = new FirebaseAuthProvider[]
                {
                    new EmailProvider(),
                    new GoogleProvider().AddScopes("email", "profile", "openid"),
                    new FacebookProvider(),
                }
            };

            AuthClient = new FirebaseAuthClient(config);

            DatabaseClient = new FirebaseClient(databaseUrl);
        }
    }
}