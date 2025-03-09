using Mingle.Entities.Models;

namespace Mingle.Shared.DTOs.Response
{
    /// <summary>
    /// Kullanıcıların alıcı olarak profil bilgilerini içeren veri transfer nesnesi (DTO).
    /// </summary>
    public sealed record RecipientProfile
    {
        public required string DisplayName { get; init; }

        public required string Email { get; init; }

        public required string Biography { get; init; }

        public required string ProfilePhoto { get; init; }

        public DateTime LastConnectionDate { get; set; }
    }
}