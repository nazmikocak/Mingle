using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mingle.Services.DTOs.Shared
{
    public sealed record ConnectionSettings
    {
        public DateTime? LastConnectionDate { get; set; }

        public List<string> ConnectionIds { get; set; } = [];
    }
}