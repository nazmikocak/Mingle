using Firebase.Auth;
using Firebase.Auth.Providers;
using Firebase.Database;
using Microsoft.Extensions.Configuration;

namespace Mingle.DataAccess.Configurations
{
    public class FirebaseConfig
    {
        public FirebaseAuthClient AuthClient { get; }
        public FirebaseClient DatabaseClient { get; }

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