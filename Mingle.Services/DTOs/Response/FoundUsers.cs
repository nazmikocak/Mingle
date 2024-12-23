using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mingle.Services.DTOs.Response
{
    public sealed record FoundUsers
    {
        public required string DisplayName { get; init; }

        public required string Email { get; init; }

        public required Uri ProfilePhoto { get; init; }
    }
}