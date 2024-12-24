namespace Mingle.Services.DTOs.Response
{
    public sealed record GroupProfile
    {
        public required string Name { get; init; }

        public required string Description { get; init; }

        public required Uri PhotoUrl { get; init; }

        public required Dictionary<string, Dictionary<string, string>> Participants { get; init; }

        public required DateTime CreatedDate { get; init; }
    }
}