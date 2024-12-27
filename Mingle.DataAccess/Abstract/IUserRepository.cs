﻿using Firebase.Database;
using Mingle.Entities.Models;

namespace Mingle.DataAccess.Abstract
{
    public interface IUserRepository
    {
        Task<IReadOnlyCollection<FirebaseObject<User>>> GetUsersAsync();

        Task CreateUserAsync(string userId, User user);

        Task<User> GetUserByIdAsync(string userId);

        Task<ConnectionSettings> GetUserConnectionStringByIdAsync(string userId);

        Task UpdateUserFieldAsync(string userId, string fieldName, object newValue);

        Task UpdateSettingsAsync(string userId, string settingsName, string fieldName, object newValue);
    }
}