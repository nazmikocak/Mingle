using AutoMapper;
using Mingle.DataAccess.Abstract;
using Mingle.Services.Abstract;
using Mingle.Services.DTOs.Request;
using Mingle.Services.DTOs.Response;
using Mingle.Services.Exceptions;
using Mingle.Services.Utilities;

namespace Mingle.Services.Concrete
{
    public sealed class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuthRepository _authRepository;
        private readonly ICloudRepository _cloudRepository;
        private readonly IMapper _mapper;


        public UserService(IUserRepository userRepository, IAuthRepository authRepository, ICloudRepository cloudRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _authRepository = authRepository;
            _cloudRepository = cloudRepository;
            _mapper = mapper;
        }


        public async Task<Dictionary<string, FoundUsers>> SearchUsersAsync(string userId, string query)
        {
            FieldValidationHelper.ValidateRequiredFields((query, "query"));

            var usersSnapshot = await _userRepository.GetAllUsersAsync();

            var users = usersSnapshot
                .Where(user =>
                    !user.Key.Equals(userId)
                    &&
                    (
                        user.Object.DisplayName.Contains(query, StringComparison.CurrentCultureIgnoreCase)
                        ||
                        user.Object.Email.Contains(query, StringComparison.CurrentCultureIgnoreCase)
                    )
                )
                .ToDictionary(
                    user => user.Key,
                    user => _mapper.Map<FoundUsers>(user.Object)
                )
                ?? throw new NotFoundException("Kullanıcı bulunamadı.");

            return users;
        }


        public async Task<UserInfo> GetUserInfoAsync(string userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId) ?? throw new NotFoundException("Kullanıcı bulunamadı");

            return _mapper.Map<UserInfo>(user);
        }


        public async Task<Dictionary<string, CallerUser>> GetUserProfilesAsync(List<string> recipientIds)
        {
            if (recipientIds.Equals(null) || recipientIds.Count.Equals(0))
            {
                throw new BadRequestException("recipientIds boş olamaz.");
            }

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


        public async Task<Uri> RemoveProfilePhotoAsync(string userId)
        {
            var defaultPhoto = new Uri("https://res.cloudinary.com/mingle-realtime-messaging-app/image/upload/v1734185072/DefaultUserProfilePhoto.png");
            await _userRepository.UpdateUserFieldAsync(userId, "ProfilePhoto", defaultPhoto);

            return defaultPhoto;
        }


        public async Task<Uri> UpdateProfilePhotoAsync(string userId, UpdateProfilePhoto dto)
        {
            var photoBytes = Convert.FromBase64String(dto.ProfilePhoto);
            var photo = new MemoryStream(photoBytes);
            FileValidationHelper.ValidatePhoto(photo);

            var newPhotoUrl = await _cloudRepository.UploadPhotoAsync(userId, $"Users", "profile_photo", photo);
            await _userRepository.UpdateUserFieldAsync(userId, "ProfilePhoto", newPhotoUrl);

            return newPhotoUrl;
        }


        public async Task UpdateDisplayNameAsync(string userId, UpdateDisplayName dto)
        {
            await _userRepository.UpdateUserFieldAsync(userId, "DisplayName", dto.DisplayName);
        }


        public async Task UpdatePhoneNumberAsync(string userId, UpdatePhoneNumber dto)
        {
            await _userRepository.UpdateUserFieldAsync(userId, "PhoneNumber", dto.PhoneNumber);
        }


        public async Task UpdateBiographyAsync(string userId, UpdateBiography dto)
        {
            await _userRepository.UpdateUserFieldAsync(userId, "Biography", dto.Biography);
        }


        public async Task ChangePasswordAsync(string userId, ChangePassword dto)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);

            var userCredential = await _authRepository.SignInUserAsync(user.Email, dto.CurrentPassword);

            await _authRepository.ChangePasswordAsync(userCredential, dto.NewPasswordAgain);
        }


        public async Task ChangeThemeAsync(string userId, ChangeTheme dto)
        {
            await _userRepository.UpdateSettingsAsync(userId, "UserSettings", "Theme", dto.Theme);
        }


        public async Task ChangeChatBackgroundAsync(string userId, ChangeChatBackground dto)
        {
            await _userRepository.UpdateSettingsAsync(userId, "UserSettings", "ChatBackground", dto.ChatBackground);
        }


        public async Task<Dictionary<string, RecipientProfile>> GetRecipientProfilesAsync(List<string> recipientIds)
        {
            //if (recipientIds.Equals(null) || recipientIds.Count.Equals(0))
            //{
            //    throw new BadRequestException("recipientIds boş olamaz.");
            //}

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


        public async Task UpdateLastConnectionDateAsync(string userId, DateTime lastConnectionDate)
        {
            await _userRepository.UpdateUserFieldAsync(userId, "LastConnectionDate", lastConnectionDate);
        }
    }
}