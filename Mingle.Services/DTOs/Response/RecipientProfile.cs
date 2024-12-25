namespace Mingle.Services.DTOs.Response
{
    public sealed record RecipientProfile
    {
        public required string DisplayName { get; init; }

        public required string Email { get; init; }

        public required string Biography { get; init; }

        public required string ProfilePhoto { get; init; }

        public DateTime? Status { get; init; }

        public DateTime? LastConnectionDate { get; set; }
    }
}