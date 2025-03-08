namespace Mingle.Entities.Models
{
    /// <summary>
    /// Sohbet (Chat) bilgilerini temsil eden sınıf.
    /// Bir sohbetin katılımcıları, mesajlar, arşiv durumu gibi bilgileri içerir.
    /// </summary>
    public sealed class Chat
    {
        public required List<string> Participants { get; set; }

        public Dictionary<string, DateTime> ArchivedFor { get; set; } = [];

        public required DateTime CreatedDate { get; set; }

        public Dictionary<string, Message> Messages { get; set; } = [];
    }
}