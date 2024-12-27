using AutoMapper;
using Mingle.Entities.Models;
using Mingle.Services.DTOs.Request;
using Mingle.Services.DTOs.Response;

namespace Mingle.Services.Mapping
{
    public sealed class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<SignUp, User>()
                .ForMember(dest => dest.Biography, opt => opt.MapFrom(src => "Merhaba, ben Mingle kullanıyorum."))
                .ForMember(dest => dest.ProfilePhoto, opt => opt.MapFrom(src => "https://res.cloudinary.com/mingle-realtime-messaging-app/image/upload/v1734185072/DefaultUserProfilePhoto.png"))
                .ForMember(dest => dest.ConnectionSettings, opt => opt.MapFrom(src => new ConnectionSettings()))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UserSettings, opt => opt.MapFrom(src => new UserSettings()));

            CreateMap<User, UserProfile>();

            CreateMap<User, FoundUsers>();

            CreateMap<User, RecipientProfile>();
        }
    }
}