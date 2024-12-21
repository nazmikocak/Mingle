using Mingle.Entities.Enums;

namespace Mingle.Entities.Models
{
    internal sealed class Message
    {
        public required string Content { get; set; }

        public required MessageContent Type { get; set; }

        public required MessageStatus Status { get; set; }

        public required Dictionary<string, DateTime> DeletedFor { get; set; }
    }
}