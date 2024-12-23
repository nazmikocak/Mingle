using Firebase.Database;
using Firebase.Database.Query;
using Mingle.DataAccess.Abstract;
using Mingle.DataAccess.Configurations;
using Mingle.Entities.Models;

namespace Mingle.DataAccess.Concrete
{
    public sealed class UserRepository : IUserRepository
    {
        private readonly FirebaseClient _databaseClient;


        public UserRepository(FirebaseConfig firebaseConfig)
        {
            _databaseClient = firebaseConfig.DatabaseClient;
        }


        public async Task<IReadOnlyCollection<FirebaseObject<User>>> GetUsersAsync()
        {
            return await _databaseClient.Child("Users").OnceAsync<User>();
        }


        public async Task CreateUserAsync(string userId, User user)
        {
            await _databaseClient.Child("Users").Child(userId).PatchAsync(user);
        }

        public async Task<User> GetUserByIdAsync(string userId)
        {
            return await _databaseClient.Child("Users").Child(userId).OnceSingleAsync<User>();
        }

        public async Task UpdateUserFieldAsync(string userId, string fieldName, object newValue)
        {
            await _databaseClient.Child("Users").Child(userId).PatchAsync(new { fieldName = newValue });
        }


    }
}