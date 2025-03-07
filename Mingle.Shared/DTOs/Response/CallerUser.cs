namespace Mingle.Shared.DTOs.Response
{
    public sealed record CallerUser
    {
        public required string DisplayName { get; init; }

        public required Uri ProfilePhoto { get; init; }

        public required DateTime LastConnectionDate { get; set; }
    }
}