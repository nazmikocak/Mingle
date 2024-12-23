namespace Mingle.Entities.Models
{
    public sealed class Chat
    {
        public required List<string> Participants { get; set; }

        public Dictionary<string, DateTime> ArchivedFor { get; set; } = [];

        public required DateTime CreatedDate { get; set; }

        public Dictionary<string, Message> Messages { get; set; } = [];
    }
}