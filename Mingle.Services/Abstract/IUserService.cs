using Firebase.Database;
using Mingle.Entities.Models;
using Mingle.Services.DTOs.Request;
using Mingle.Services.DTOs.Response;

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

        Task<Dictionary<string, RecipientProfile>> GetRecipientProfilesAsync(List<string> recipientIds);

        Task<ConnectionSettings> GetConnectionSettingsAsync(string userId);

        Task SaveConnectionSettingsAsync(string userId, ConnectionSettings dto);

        Task<List<List<string>>> GetUserConnectionIdsAsync(List<string> userIds);
    }
}