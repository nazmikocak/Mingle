using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mingle.Entities.Models
{
    public sealed class MessageStatus
    {
        public required Dictionary<string, DateTime> Sent { get; set; }

        public Dictionary<string, DateTime>? Delivered { get; set; } = [];

        public Dictionary<string, DateTime>? Seen { get; set; } = [];
    }
}