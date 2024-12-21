using AutoMapper;
using Mingle.Entities.Models;
using Mingle.Services.DTOs;

namespace Mingle.Services.Mapping
{
    public sealed class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<SignUpRequest, User>()
                .ForMember(dest => dest.Biography, opt => opt.MapFrom(src => "Merhaba, ben Mingle kullanıyorum."))
                .ForMember(dest => dest.ProfilePhoto, opt => opt.MapFrom(src => "https://res.cloudinary.com/mingle-realtime-messaging-app/image/upload/v1734185072/DefaultUserProfilePhoto.png"))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Settings, opt => opt.MapFrom(src => new UserSettings()));

            CreateMap<User, UserProfileResponse>();
        }
    }
}