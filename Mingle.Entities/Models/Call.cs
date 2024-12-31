using Mingle.Entities.Enums;

namespace Mingle.Entities.Models
{
    public class Call
    {
        public required List<string> Participants { get; set; }

        public required string CallInitiator { get; set; }

        public required CallType Type { get; set; }

        public required CallStatus Status { get; set; }

        public DateTime? CallDuration { get; set; }

        public required DateTime CreatedDate { get; set; }
    }
}