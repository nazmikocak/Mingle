namespace Mingle.Services.DTOs
{
    public sealed record UserProfileResponse
    {
        public required string DisplayName { get; init; }

        public required string Email { get; init; }

        public string? PhoneNumber { get; init; }

        public required string Biography { get; init; }

        public required Uri ProfilePhoto { get; init; }
    }
}