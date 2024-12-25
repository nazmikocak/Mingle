using Mingle.Entities.Models;

namespace Mingle.Services.DTOs.Response
{
    public sealed record ChatPreview
    {
        public required Uri Photo { get; init; }

        public required string Name { get; init; }

        public required Message LastMessage { get; init; }
    }
}