using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mingle.Entities.Models
{
    internal sealed class Group
    {
        public required string Name { get; set; }

        public required string Description { get; set; }

        public required string Photo { get; set; }

        public required Dictionary<string, string> Participants { get; set; }

        public required string CreatedBy { get; set; }

        public required DateTime CreatedDate { get; set; }
    }
}