using Mingle.Entities.Models;

namespace Mingle.Shared.DTOs.Response
{
    /// <summary>
    /// Kullanıcının profil bilgileri, iletişim bilgileri ve hesap ayarlarını içeren veri transfer nesnesi (DTO).
    /// </summary>
    public sealed record UserInfo
    {
        public required string DisplayName { get; init; }

        public required string Email { get; init; }

        public string? PhoneNumber { get; init; }

        public required string Biography { get; init; }

        public required string ProviderId { get; init; }

        public required Uri ProfilePhoto { get; init; }

        public required UserSettings UserSettings { get; set; }
    }
}