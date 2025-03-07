namespace Mingle.Shared.DTOs.Response
{
    public sealed record FoundUsers
    {
        public required string DisplayName { get; init; }

        public required string Email { get; init; }

        public required Uri ProfilePhoto { get; init; }
    }
}