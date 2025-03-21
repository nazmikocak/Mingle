using Mingle.Shared.DTOs.Request;
using Mingle.Shared.DTOs.Response;

namespace Mingle.Services.Abstract
{
    /// <summary>
    /// Kullanıcı yönetim servislerini sağlayan arayüz.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Kullanıcıları sorgular ve eşleşenleri döner.
        /// </summary>
        /// <param name="userId">Arama yapan kullanıcının kimliği.</param>
        /// <param name="query">Arama sorgusu.</param>
        /// <returns>Eşleşen kullanıcıları içeren bir sözlük döner.</returns>
        Task<Dictionary<string, FoundUsers>> SearchUsersAsync(string userId, string query);



        /// <summary>
        /// Kullanıcı bilgilerini getirir.
        /// </summary>
        /// <param name="userId">Bilgisi getirilecek kullanıcının kimliği.</param>
        /// <returns>Kullanıcı bilgilerini içeren bir DTO döner.</returns>
        Task<UserInfo> GetUserInfoAsync(string userId);



        /// <summary>
        /// Birden fazla kullanıcı profili getirir.
        /// </summary>
        /// <param name="recipientIds">Kullanıcı profilleri için kimlikler listesi.</param>
        /// <returns>Kullanıcı profillerini içeren bir sözlük döner.</returns>
        Task<Dictionary<string, CallerUser>> GetUserProfilesAsync(List<string> recipientIds);



        /// <summary>
        /// Kullanıcı profil fotoğrafını kaldırır.
        /// </summary>
        /// <param name="userId">Profil fotoğrafı kaldırılacak kullanıcının kimliği.</param>
        /// <returns>Kaldırılmış profil fotoğrafının URI'sini döner.</returns>
        Task<Uri> RemoveProfilePhotoAsync(string userId);



        /// <summary>
        /// Kullanıcı profil fotoğrafını günceller.
        /// </summary>
        /// <param name="userId">Profil fotoğrafı güncellenecek kullanıcının kimliği.</param>
        /// <param name="dto">Yeni profil fotoğrafını içeren DTO.</param>
        /// <returns>Güncellenmiş profil fotoğrafının URI'sini döner.</returns>
        Task<Uri> UpdateProfilePhotoAsync(string userId, UpdateProfilePhoto dto);



        /// <summary>
        /// Kullanıcı adı (DisplayName) günceller.
        /// </summary>
        /// <param name="userId">Kullanıcı adını güncelleyecek kullanıcının kimliği.</param>
        /// <param name="dto">Yeni kullanıcı adı bilgilerini içeren DTO.</param>
        /// <returns>Bir işlem sonucu döner (void).</returns>
        Task UpdateDisplayNameAsync(string userId, UpdateDisplayName dto);



        /// <summary>
        /// Kullanıcı telefon numarasını günceller.
        /// </summary>
        /// <param name="userId">Telefon numarası güncellenecek kullanıcının kimliği.</param>
        /// <param name="dto">Yeni telefon numarasını içeren DTO.</param>
        /// <returns>Bir işlem sonucu döner (void).</returns>
        Task UpdatePhoneNumberAsync(string userId, UpdatePhoneNumber dto);



        /// <summary>
        /// Kullanıcı biyografisini günceller.
        /// </summary>
        /// <param name="userId">Biyografi güncellenecek kullanıcının kimliği.</param>
        /// <param name="dto">Yeni biyografi bilgisini içeren DTO.</param>
        /// <returns>Bir işlem sonucu döner (void).</returns>
        Task UpdateBiographyAsync(string userId, UpdateBiography dto);



        /// <summary>
        /// Kullanıcı şifresini değiştirir.
        /// </summary>
        /// <param name="userId">Şifre değişikliği yapılacak kullanıcının kimliği.</param>
        /// <param name="dto">Yeni şifreyi içeren DTO.</param>
        /// <returns>Bir işlem sonucu döner (void).</returns>
        Task ChangePasswordAsync(string userId, ChangePassword dto);



        /// <summary>
        /// Kullanıcı temasını değiştirir.
        /// </summary>
        /// <param name="userId">Tema değişikliği yapılacak kullanıcının kimliği.</param>
        /// <param name="dto">Yeni tema bilgilerini içeren DTO.</param>
        /// <returns>Bir işlem sonucu döner (void).</returns>
        Task ChangeThemeAsync(string userId, ChangeTheme dto);



        /// <summary>
        /// Kullanıcı sohbet arka planını değiştirir.
        /// </summary>
        /// <param name="userId">Sohbet arka planı değişikliği yapılacak kullanıcının kimliği.</param>
        /// <param name="dto">Yeni sohbet arka planını içeren DTO.</param>
        /// <returns>Bir işlem sonucu döner (void).</returns>
        Task ChangeChatBackgroundAsync(string userId, ChangeChatBackground dto);



        /// <summary>
        /// Birden fazla alıcının profil bilgilerini getirir.
        /// </summary>
        /// <param name="recipientIds">Alıcıların kimlikleri.</param>
        /// <returns>Alıcı profillerini içeren bir sözlük döner.</returns>
        Task<Dictionary<string, RecipientProfile>> GetRecipientProfilesAsync(List<string> recipientIds);



        /// <summary>
        /// Kullanıcının son bağlantı tarihini günceller.
        /// </summary>
        /// <param name="userId">Bağlantı tarihi güncellenecek kullanıcının kimliği.</param>
        /// <param name="lastConnectionDate">Yeni bağlantı tarihi.</param>
        /// <returns>Bir işlem sonucu döner (void).</returns>
        Task UpdateLastConnectionDateAsync(string userId, DateTime lastConnectionDate);
    }
}
