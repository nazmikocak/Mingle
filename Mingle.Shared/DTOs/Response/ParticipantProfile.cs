using Mingle.Entities.Enums;

namespace Mingle.Shared.DTOs.Response
{
    /// <summary>
    /// Gruptaki katılımcının rolü ve temel bilgilerini içeren veri transfer nesnesi (DTO).
    /// </summary>
    public sealed record ParticipantProfile
    {
        public required string DisplayName { get; init; }

        public required GroupParticipant Role { get; init; }

        public required Uri ProfilePhoto { get; init; }

        public DateTime? LastConnectionDate { get; set; }
    }
}