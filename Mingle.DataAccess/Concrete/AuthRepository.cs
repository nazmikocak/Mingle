using Firebase.Auth;
using Mingle.DataAccess.Abstract;
using Mingle.DataAccess.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mingle.DataAccess.Concrete
{
    public sealed class AuthRepository : IAuthRepository
    {
        private readonly FirebaseAuthClient _authClient;

        public AuthRepository(FirebaseConfiguration firebaseConfiguration)
        {
            _authClient = firebaseConfiguration.AuthClient;
        }

        public async Task<UserCredential> SignUpUserAsync(string email, string password, string displayName) 
        {
            var userCredential = await _authClient.CreateUserWithEmailAndPasswordAsync(email, password, displayName);
            return userCredential;
        }

        public async Task<UserCredential> SignInUserAsync(string email, string password) 
        {
            var userCredential = await _authClient.SignInWithEmailAndPasswordAsync(email, password);
            return userCredential;
        }
    }
}