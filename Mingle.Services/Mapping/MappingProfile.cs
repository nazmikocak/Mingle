using AutoMapper;
using Mingle.Entities.Models;
using Mingle.Shared.DTOs.Request;
using Mingle.Shared.DTOs.Response;
using User = Mingle.Entities.Models.User;
using UserInfo = Mingle.Shared.DTOs.Response.UserInfo;

namespace Mingle.Services.Mapping
{
    /// <summary>
    /// AutoMapper profil sınıfı, nesne dönüştürme işlemleri için harita tanımlar.
    /// </summary>
    public sealed class MappingProfile : Profile
    {
        /// <summary>
        /// MappingProfile sınıfının yeni bir örneğini oluşturur ve nesneler arasındaki dönüşüm haritalarını yapılandırır.
        /// </summary>
        public MappingProfile()
        {
            // SignUp => User
            CreateMap<SignUp, User>()
                .ForMember(dest => dest.Biography, opt => opt.MapFrom(src => "Merhaba, ben Mingle kullanıyorum."))
                .ForMember(dest => dest.ProfilePhoto, opt => opt.MapFrom(src => "https://res.cloudinary.com/mingle-realtime-messaging-app/image/upload/v1744980054/DefaultUserProfilePhoto.png"))
                .ForMember(dest => dest.ProviderId, opt => opt.MapFrom(src => "email"))
                .ForMember(dest => dest.LastConnectionDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UserSettings, opt => opt.MapFrom(src => new UserSettings()))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow));


            // ProviderData => User
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


            // User => UserInfo
            CreateMap<User, UserInfo>();


            // User => FoundUsers
            CreateMap<User, FoundUsers>();


            // User => RecipientProfile
            CreateMap<User, RecipientProfile>();


            // User => CallerUser
            CreateMap<User, CallerUser>();
        }
    }
}