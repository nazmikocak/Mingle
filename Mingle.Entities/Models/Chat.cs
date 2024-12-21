using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mingle.Entities.Models
{
    internal sealed class Chat
    {
        public required List<string> Participants { get; set; }

        public Dictionary<string, DateTime> ArchivedFor { get; set; } = [];

        public required DateTime CreatedDate { get; set; }

        public Dictionary<string, Message> Messages { get; set; } = [];
    }
}