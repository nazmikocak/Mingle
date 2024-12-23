using Mingle.Entities.Enums;

namespace Mingle.Entities.Models
{
    public sealed class Message
    {
        public required string Content { get; set; }

        public required MessageContent Type { get; set; }

        public required MessageStatus Status { get; set; }

        public Dictionary<string, DateTime>? DeletedFor { get; set; } = [];
    }
}