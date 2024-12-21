﻿using Firebase.Auth;
using Firebase.Auth.Providers;
using Firebase.Database;
using Microsoft.Extensions.Configuration;

namespace Mingle.DataAccess.Configurations
{
    public class FirebaseConfiguration
    {
        public FirebaseAuthClient AuthClient { get; }
        public FirebaseClient DatabaseClient { get; }

        public FirebaseConfiguration(IConfiguration configuration)
        {
            var firebaseApiKey = configuration["FirebaseSettings:ApiKey"];
            var authDomain = configuration["FirebaseSettings:AuthDomain"];
            var databaseUrl = configuration["FirebaseSettings:DatabaseUrl"];

            var config = new FirebaseAuthConfig
            {
                ApiKey = firebaseApiKey,
                AuthDomain = authDomain,
                Providers = new FirebaseAuthProvider[]
                {
                    new EmailProvider()
                }
            };

            AuthClient = new FirebaseAuthClient(config);

            DatabaseClient = new FirebaseClient(databaseUrl);
        }
    }
}