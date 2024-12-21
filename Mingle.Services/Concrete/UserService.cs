using AutoMapper;
using Mingle.DataAccess.Abstract;
using Mingle.Services.Abstract;
using Mingle.Services.DTOs;

namespace Mingle.Services.Concrete
{
    public sealed class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }


        public async Task<UserProfileResponse> GetUserProfileAsync(string userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);

            var userProfile = _mapper.Map<UserProfileResponse>(user);
            return userProfile;
        }


        public async Task RemoveProfilePhotoAsync(string userId)
        {
            var defaultPhoto = new Uri("https://res.cloudinary.com/mingle-realtime-messaging-app/image/upload/v1734185072/DefaultUserProfilePhoto.png");
            await _userRepository.UpdateUserFieldAsync(userId, "ProfilePhoto", defaultPhoto);
        }
    }
}
