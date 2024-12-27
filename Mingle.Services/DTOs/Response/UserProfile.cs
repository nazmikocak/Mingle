using Mingle.Entities.Models;

namespace Mingle.Services.DTOs.Response
{
    public sealed record UserProfile
    {
        public required string DisplayName { get; init; }

        public required string Email { get; init; }

        public string? PhoneNumber { get; init; }

        public required string Biography { get; init; }

        public required Uri ProfilePhoto { get; init; }

        public required UserSettings UserSettings { get; set; }
    }
}