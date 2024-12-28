namespace Mingle.Entities.Models
{
    public sealed class MessageStatus
    {
        public required Dictionary<string, DateTime> Sent { get; set; }

        public Dictionary<string, DateTime> Delivered { get; set; } = [];

        public Dictionary<string, DateTime> Read { get; set; } = [];
    }
}