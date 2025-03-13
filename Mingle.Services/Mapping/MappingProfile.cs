using AutoMapper;
using Firebase.Auth;
using Mingle.Entities.Models;
using Mingle.Shared.DTOs.Request;
using Mingle.Shared.DTOs.Response;
using User = Mingle.Entities.Models.User;
using UserInfo = Mingle.Shared.DTOs.Response.UserInfo;



namespace Mingle.Services.Mapping
{
    public sealed class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<SignUp, User>()
                .ForMember(dest => dest.Biography, opt => opt.MapFrom(src => "Merhaba, ben Mingle kullanıyorum."))
                .ForMember(dest => dest.ProfilePhoto, opt => opt.MapFrom(src => "https://res.cloudinary.com/mingle-realtime-messaging-app/image/upload/v1734185072/DefaultUserProfilePhoto.png"))
                .ForMember(dest => dest.ProviderId, opt => opt.MapFrom(src => "email"))
                .ForMember(dest => dest.LastConnectionDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UserSettings, opt => opt.MapFrom(src => new UserSettings()))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<ProviderData, User>()
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.DisplayName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.Biography, opt => opt.MapFrom(src => "Merhaba, ben Mingle kullanıyorum."))
                .ForMember(dest => dest.ProfilePhoto, opt => opt.MapFrom(src => new Uri(src.PhotoURL)))
                .ForMember(dest => dest.LastConnectionDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => DateTime.MinValue))
                .ForMember(dest => dest.UserSettings, opt => opt.MapFrom(src => new UserSettings()))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<User, UserInfo>();

            CreateMap<User, FoundUsers>();

            CreateMap<User, RecipientProfile>();

            CreateMap<User, CallerUser>();
        }
    }
}