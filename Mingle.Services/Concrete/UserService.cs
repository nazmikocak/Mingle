using AutoMapper;
using Firebase.Database;
using Mingle.DataAccess.Abstract;
using Mingle.Entities.Models;
using Mingle.Services.Abstract;
using Mingle.Services.DTOs.Request;
using Mingle.Services.DTOs.Response;
using Mingle.Services.DTOs.Shared;
using Mingle.Services.Exceptions;
using Mingle.Services.Utilities;
using System.Text.RegularExpressions;

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


        public async Task<Dictionary<string, FoundUsers>> SearchUsersAsync(string userId, SearchedUsers dto)
        {
            var usersSnapshot = await _userRepository.GetUsersAsync();

            var users = usersSnapshot
                .Where(user =>
                    !user.Key.Equals(userId)
                    &&
                    (
                        user.Object.DisplayName.Contains(dto.Query, StringComparison.CurrentCultureIgnoreCase)
                        ||
                        user.Object.Email.Contains(dto.Query, StringComparison.CurrentCultureIgnoreCase)
                    )
                )
                .ToDictionary(
                    user => user.Key,
                    user => _mapper.Map<FoundUsers>(user.Object)
                );


            if (users == null || users.Count == 0)
            {
                throw new NotFoundException("Kullanıcı bulunamadı.");
            }
            else
            {
                return users;
            }
        }


        public async Task<UserProfile> GetUserProfileAsync(string userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);

            var userProfile = _mapper.Map<UserProfile>(user);
            return userProfile;
        }


        public async Task<Uri> RemoveProfilePhotoAsync(string userId)
        {
            var defaultPhoto = new Uri("https://res.cloudinary.com/mingle-realtime-messaging-app/image/upload/v1734185072/DefaultUserProfilePhoto.png");

            await _userRepository.UpdateUserFieldAsync(userId, "ProfilePhoto", defaultPhoto);
            return defaultPhoto;
        }


        public async Task<Uri> UpdateProfilePhotoAsync(string userId, UpdateProfilePhoto dto)
        {
            var photo = new MemoryStream(dto.ProfilePhoto);
            FileValidationHelper.ValidateProfilePhoto(photo);

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
            await _userRepository.UpdateUserSettingsAsync(userId, "Theme", dto.Theme);
        }


        public async Task ChangeChatBackgroundAsync(string userId, ChangeChatBackground dto)
        {
            await _userRepository.UpdateUserSettingsAsync(userId, "ChatBackground", dto.ChatBackground);
        }


        public async Task<RecipientProfile> GetRecipientProfileAsync(string recipientId)
        {
            var user = await _userRepository.GetUserByIdAsync(recipientId) ?? throw new NotFoundException("Kullanıcı bulunamadı.");

            var recipientProfile = _mapper.Map<RecipientProfile>(user);

            return recipientProfile;
        }


        public async Task<ConnectionSettings> GetConnectionSettingsAsync(string userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);

            return _mapper.Map<ConnectionSettings>(user);
        }


        public async Task SaveConnectionSettingsAsync(string userId, ConnectionSettings dto)
        {
            await _userRepository.UpdateUserFieldAsync(userId, "LastConnectionDate", dto.LastConnectionDate!);

            await _userRepository.UpdateUserFieldAsync(userId, "ConnectionIds", dto.ConnectionIds);
        }
    }
}