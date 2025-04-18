using AutoMapper;
using Mingle.DataAccess.Abstract;
using Mingle.Services.Abstract;
using Mingle.Services.Exceptions;
using Mingle.Services.Utilities;
using Mingle.Shared.DTOs.Request;
using Mingle.Shared.DTOs.Response;

namespace Mingle.Services.Concrete
{
    /// <summary>
    /// Kullanıcı yönetimi işlemlerini yöneten servis sınıfıdır.
    /// Kullanıcı profili güncellemeleri, fotoğraf yüklemeleri ve arama işlemleri gibi kullanıcı ile ilgili işlemleri içerir.
    /// </summary>
    public sealed class UserService : IUserService
    {
        private readonly ICloudRepository _cloudRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAuthRepository _authRepository;
        private readonly IMapper _mapper;



        /// <summary>
        /// UserService sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        /// <param name="cloudRepository">Bulut işlemleri için kullanılan repository.</param>
        /// <param name="userRepository">Kullanıcı yönetimi için kullanılan repository.</param>
        /// <param name="authRepository">Kimlik doğrulama işlemleri için kullanılan repository.</param>
        /// <param name="mapper">DTO ile varlık nesneleri arasında dönüşüm sağlayan AutoMapper.</param>
        public UserService(ICloudRepository cloudRepository, IUserRepository userRepository, IAuthRepository authRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _authRepository = authRepository;
            _cloudRepository = cloudRepository;
            _mapper = mapper;
        }



        /// <summary>
        /// Kullanıcıları arar ve sonuçları döndürür.
        /// </summary>
        /// <param name="userId">Arama yapan kullanıcının ID'si.</param>
        /// <param name="query">Arama sorgusu.</param>
        /// <returns>Arama sonucu bulunan kullanıcıları içeren bir sözlük.</returns>
        /// <exception cref="NotFoundException">Arama sonucunda kullanıcı bulunamazsa fırlatılır.</exception>
        public async Task<Dictionary<string, FoundUsers>> SearchUsersAsync(string userId, string query)
        {
            FieldValidationHelper.ValidateRequiredFields((query, "query"));

            var usersSnapshot = await _userRepository.GetAllUsersAsync();

            var users = usersSnapshot
                .Where(user =>
                    !user.Key.Equals(userId)
                    &&
                    (
                        (user.Object.DisplayName?.Contains(query, StringComparison.CurrentCultureIgnoreCase) ?? false)
                        ||
                        (user.Object.Email?.Contains(query, StringComparison.CurrentCultureIgnoreCase) ?? false)
                    )
                )
                .ToDictionary(
                    user => user.Key,
                    user => _mapper.Map<FoundUsers>(user.Object)
                )
                ?? throw new NotFoundException("Kullanıcı bulunamadı.");

            return users;
        }



        /// <summary>
        /// Kullanıcı bilgilerini döndürür.
        /// </summary>
        /// <param name="userId">Bilgileri alınacak kullanıcının ID'si.</param>
        /// <returns>Kullanıcı bilgilerini içeren bir <see cref="UserInfo"/> nesnesi.</returns>
        /// <exception cref="NotFoundException">Kullanıcı bulunamazsa fırlatılır.</exception>
        public async Task<UserInfo> GetUserInfoAsync(string userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId) ?? throw new NotFoundException("Kullanıcı bulunamadı");

            return _mapper.Map<UserInfo>(user);
        }



        /// <summary>
        /// Kullanıcı profil bilgilerini döndürür.
        /// </summary>
        /// <param name="recipientIds">Profil bilgileri alınacak kullanıcıların ID listesi.</param>
        /// <returns>Kullanıcı profillerini içeren bir sözlük.</returns>
        /// <exception cref="NotFoundException">Kullanıcılar bulunamazsa fırlatılır.</exception>
        public async Task<Dictionary<string, CallerUser>> GetUserProfilesAsync(List<string> recipientIds)
        {
            var users = await _userRepository.GetAllUsersAsync();

            var recipientProfiles = users
                .Where(user => recipientIds.Contains(user.Key))
                .ToDictionary(
                    user => user.Key,
                    user => _mapper.Map<CallerUser>(user.Object)
                )
                ?? throw new NotFoundException("Kullanıcı bulunamadı.");

            return recipientProfiles;
        }



        /// <summary>
        /// Kullanıcı profil fotoğrafını siler ve varsayılan fotoğrafla değiştirir.
        /// </summary>
        /// <param name="userId">Fotoğrafı silinecek kullanıcının ID'si.</param>
        /// <returns>Varsayılan fotoğraf URL'sini içeren bir <see cref="Uri"/>.</returns>
        public async Task<Uri> RemoveProfilePhotoAsync(string userId)
        {
            var defaultPhoto = new Uri("https://res.cloudinary.com/mingle-realtime-messaging-app/image/upload/v1744980054/DefaultUserProfilePhoto_bfx3gw.png");
            await _userRepository.UpdateUserFieldAsync(userId, "ProfilePhoto", defaultPhoto);

            return defaultPhoto;
        }



        /// <summary>
        /// Kullanıcının profil fotoğrafını günceller.
        /// </summary>
        /// <param name="userId">Fotoğrafı güncellenen kullanıcının ID'si.</param>
        /// <param name="dto">Yeni fotoğraf URL'sini içeren DTO.</param>
        /// <returns>Yeni profil fotoğrafının URL'sini içeren bir <see cref="Uri"/>.</returns>
        public async Task<Uri> UpdateProfilePhotoAsync(string userId, UpdateProfilePhoto dto)
        {
            var newPhotoUrl = await _cloudRepository.UploadPhotoAsync(userId, $"Users", "profile_photo", FileValidationHelper.ValidatePhoto(dto.ProfilePhoto));
            await _userRepository.UpdateUserFieldAsync(userId, "ProfilePhoto", newPhotoUrl);

            return newPhotoUrl;
        }



        /// <summary>
        /// Kullanıcının ekran adını günceller.
        /// </summary>
        /// <param name="userId">Ekran adı güncellenen kullanıcının ID'si.</param>
        /// <param name="dto">Yeni ekran adı içeren DTO.</param>
        public async Task UpdateDisplayNameAsync(string userId, UpdateDisplayName dto)
        {
            await _userRepository.UpdateUserFieldAsync(userId, "DisplayName", dto.DisplayName);
        }



        /// <summary>
        /// Kullanıcının telefon numarasını günceller.
        /// </summary>
        /// <param name="userId">Telefon numarası güncellenen kullanıcının ID'si.</param>
        /// <param name="dto">Yeni telefon numarasını içeren DTO.</param>
        public async Task UpdatePhoneNumberAsync(string userId, UpdatePhoneNumber dto)
        {
            await _userRepository.UpdateUserFieldAsync(userId, "PhoneNumber", dto.PhoneNumber);
        }



        /// <summary>
        /// Kullanıcının biyografisini günceller.
        /// </summary>
        /// <param name="userId">Biyografi güncellenen kullanıcının ID'si.</param>
        /// <param name="dto">Yeni biyografi içeren DTO.</param>
        public async Task UpdateBiographyAsync(string userId, UpdateBiography dto)
        {
            await _userRepository.UpdateUserFieldAsync(userId, "Biography", dto.Biography);
        }



        /// <summary>
        /// Kullanıcının şifresini değiştirir.
        /// </summary>
        /// <param name="userId">Şifre değişikliği yapılacak kullanıcının ID'si.</param>
        /// <param name="dto">Yeni şifre bilgilerini içeren DTO.</param>
        public async Task ChangePasswordAsync(string userId, ChangePassword dto)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);

            var userCredential = await _authRepository.SignInWithEmailAsync(user.Email, dto.CurrentPassword);

            await _authRepository.ChangePasswordAsync(userCredential, dto.NewPasswordAgain);
        }



        /// <summary>
        /// Kullanıcının tema tercihini değiştirir.
        /// </summary>
        /// <param name="userId">Tema tercihi değiştirilen kullanıcının ID'si.</param>
        /// <param name="dto">Yeni tema bilgilerini içeren DTO.</param>
        public async Task ChangeThemeAsync(string userId, ChangeTheme dto)
        {
            await _userRepository.UpdateSettingsAsync(userId, "UserSettings", "Theme", dto.Theme);
        }



        /// <summary>
        /// Kullanıcının sohbet arka planını değiştirir.
        /// </summary>
        /// <param name="userId">Arka planı değiştirilen kullanıcının ID'si.</param>
        /// <param name="dto">Yeni arka plan bilgilerini içeren DTO.</param>
        public async Task ChangeChatBackgroundAsync(string userId, ChangeChatBackground dto)
        {
            await _userRepository.UpdateSettingsAsync(userId, "UserSettings", "ChatBackground", dto.ChatBackground);
        }



        /// <summary>
        /// Kullanıcı profil bilgilerini döndürür.
        /// </summary>
        /// <param name="recipientIds">Profil bilgileri alınacak kullanıcıların ID listesi.</param>
        /// <returns>Kullanıcı profillerini içeren bir sözlük.</returns>
        /// <exception cref="NotFoundException">Kullanıcılar bulunamazsa fırlatılır.</exception>
        public async Task<Dictionary<string, RecipientProfile>> GetRecipientProfilesAsync(List<string> recipientIds)
        {
            var users = await _userRepository.GetAllUsersAsync();

            var recipientProfiles = users
                .Where(user => recipientIds.Contains(user.Key))
                .ToDictionary(
                    user => user.Key,
                    user => _mapper.Map<RecipientProfile>(user.Object)
                )
                ?? throw new NotFoundException("Kullanıcı bulunamadı.");

            return recipientProfiles;
        }



        /// <summary>
        /// Kullanıcının son bağlantı tarihini günceller.
        /// </summary>
        /// <param name="userId">Bağlantı tarihi güncellenen kullanıcının ID'si.</param>
        /// <param name="lastConnectionDate">Yeni bağlantı tarihi.</param>
        public async Task UpdateLastConnectionDateAsync(string userId, DateTime lastConnectionDate)
        {
            await _userRepository.UpdateUserFieldAsync(userId, "LastConnectionDate", lastConnectionDate);
        }
    }
}