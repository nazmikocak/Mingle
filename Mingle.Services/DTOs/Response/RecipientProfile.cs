using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mingle.Services.DTOs.Response
{
    public sealed record RecipientProfile
    {
        public required string DisplayName { get; init; }

        public required string Email { get; init; }

        public required string Biography { get; init; }

        public required string ProfilePhoto { get; init; }

        public DateTime? Status { get; init; }
    }
}
