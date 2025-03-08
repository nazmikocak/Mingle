using Mingle.Entities.Enums;

namespace Mingle.Entities.Models
{
    /// <summary>
    /// Çağrı (Call) bilgilerini temsil eden sınıf.
    /// Bir çağrıdaki katılımcılar, çağrı tipi, durumu, süresi gibi bilgileri içerir.
    /// </summary>
    public sealed class Call
    {
        public required List<string> Participants { get; set; }

        public required CallType Type { get; set; }

        public required CallStatus Status { get; set; }

        public TimeSpan? CallDuration { get; set; }

        public Dictionary<string, DateTime>? DeletedFor { get; set; } = [];

        public required DateTime CreatedDate { get; set; }
    }
}