namespace Mingle.Services.DTOs.Response
{
    public sealed record CallerUser
    {
        public required string DisplayName { get; init; }

        public required Uri ProfilePhoto { get; init; }
    }
}