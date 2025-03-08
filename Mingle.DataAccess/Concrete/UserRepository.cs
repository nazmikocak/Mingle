﻿using Firebase.Database;
using Firebase.Database.Query;
using Microsoft.Extensions.Options;
using Mingle.DataAccess.Abstract;
using Mingle.DataAccess.Configurations;
using Mingle.Entities.Models;

namespace Mingle.DataAccess.Concrete
{
    public sealed class UserRepository : IUserRepository
    {
        private readonly FirebaseClient _databaseClient;


        public UserRepository(IOptions<FirebaseConfig> firebaseConfig)
        {
            _databaseClient = firebaseConfig.Value.DatabaseClient;
        }


        public async Task<IReadOnlyCollection<FirebaseObject<User>>> GetAllUsersAsync()
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
            var fieldData = new Dictionary<string, object>
            {
                { fieldName, newValue }
            };

            await _databaseClient.Child("Users").Child(userId).PatchAsync(fieldData);
        }


        public async Task UpdateSettingsAsync(string userId, string settingsName, string fieldName, object newValue)
        {
            var fieldData = new Dictionary<string, object>
            {
                { fieldName, newValue }
            };

            await _databaseClient.Child("Users").Child(userId).Child(settingsName).PatchAsync(fieldData);
        }
    }
}