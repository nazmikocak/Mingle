using Mingle.Shared.DTOs.Request;
using Mingle.Shared.DTOs.Response;
namespace Mingle.Services.Abstract
{
    public interface IUserService
    {
        Task<Dictionary<string, FoundUsers>> SearchUsersAsync(string userId, string query);

        Task<UserInfo> GetUserInfoAsync(string userId);

        Task<Dictionary<string, CallerUser>> GetUserProfilesAsync(List<string> recipientIds);

        Task<Uri> RemoveProfilePhotoAsync(string userId);

        Task<Uri> UpdateProfilePhotoAsync(string userId, UpdateProfilePhoto dto);

        Task UpdateDisplayNameAsync(string userId, UpdateDisplayName dto);

        Task UpdatePhoneNumberAsync(string userId, UpdatePhoneNumber dto);

        Task UpdateBiographyAsync(string userId, UpdateBiography dto);

        Task ChangePasswordAsync(string userId, ChangePassword dto);

        Task ChangeThemeAsync(string userId, ChangeTheme dto);

        Task ChangeChatBackgroundAsync(string userId, ChangeChatBackground dto);

        Task<Dictionary<string, RecipientProfile>> GetRecipientProfilesAsync(List<string> recipientIds);

        Task UpdateLastConnectionDateAsync(string userId, DateTime lastConnectionDate);
    }
}