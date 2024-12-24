using Firebase.Database;
using Mingle.Entities.Models;
using Mingle.Services.DTOs.Request;
using Mingle.Services.DTOs.Response;
using Mingle.Services.DTOs.Shared;

namespace Mingle.Services.Abstract
{
    public interface IUserService
    {
        Task<Dictionary<string, FoundUsers>> SearchUsersAsync(string userId, SearchedUsers dto);

        Task<UserProfile> GetUserProfileAsync(string userId);

        Task<Uri> RemoveProfilePhotoAsync(string userId);

        Task<Uri> UpdateProfilePhotoAsync(string userId, UpdateProfilePhoto dto);

        Task UpdateDisplayNameAsync(string userId, UpdateDisplayName dto);

        Task UpdatePhoneNumberAsync(string userId, UpdatePhoneNumber dto);

        Task UpdateBiographyAsync(string userId, UpdateBiography dto);

        Task ChangePasswordAsync(string userId, ChangePassword dto);

        Task ChangeThemeAsync(string userId, ChangeTheme dto);

        Task ChangeChatBackgroundAsync(string userId, ChangeChatBackground dto);

        Task<RecipientProfile> GetRecipientProfileAsync(string recipientId);

        Task<ConnectionSettings> GetConnectionSettingsAsync(string userId);

        Task SaveConnectionSettingsAsync(string userId, ConnectionSettings dto);
    }
}