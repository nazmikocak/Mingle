using Mingle.Entities.Enums;

namespace Mingle.Shared.DTOs.Response
{
    public sealed record ParticipantProfile
    {
        public required string DisplayName { get; init; }

        public required GroupParticipant Role { get; init; }

        public required Uri ProfilePhoto { get; init; }

        public DateTime? LastConnectionDate { get; set; }
    }
}