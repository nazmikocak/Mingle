using Mingle.Entities.Enums;

namespace Mingle.Entities.Models
{
    /// <summary>
    /// Mesaj bilgilerini temsil eden sınıf.
    /// Bir mesajın içeriği, tipi, durumu ve silindiği kullanıcılar gibi bilgileri içerir.
    /// </summary>
    public sealed class Message
    {
        public required string Content { get; set; }

        public required MessageContent Type { get; set; }

        public required MessageStatus Status { get; set; }

        public Dictionary<string, DateTime>? DeletedFor { get; set; } = [];
    }
}