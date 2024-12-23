using AutoMapper;
using Mingle.Entities.Models;
using Mingle.Services.DTOs.Request;
using Mingle.Services.DTOs.Response;
using Mingle.Services.DTOs.Shared;

namespace Mingle.Services.Mapping
{
    public sealed class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<SignUp, User>()
                .ForMember(dest => dest.Biography, opt => opt.MapFrom(src => "Merhaba, ben Mingle kullanıyorum."))
                .ForMember(dest => dest.ProfilePhoto, opt => opt.MapFrom(src => "https://res.cloudinary.com/mingle-realtime-messaging-app/image/upload/v1734185072/DefaultUserProfilePhoto.png"))
                .ForMember(dest => dest.LastConnectionDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Settings, opt => opt.MapFrom(src => new UserSettings()));

            CreateMap<User, UserProfile>();

            CreateMap<User, FoundUsers>();

            CreateMap<User, RecipientProfile>();

            CreateMap<User, ConnectionSettings>()
                .ForMember(dest => dest.LastConnectionDate, opt => opt.MapFrom(src => src.LastConnectionDate))
                .ForMember(dest => dest.ConnectionIds, opt => opt.MapFrom(src => src.ConnectionIds));
        }
    }
}