namespace Mingle.Entities.Models
{
    public sealed class User
    {
        public required string DisplayName { get; set; }

        public required string Email { get; set; }

        public string PhoneNumber { get; set; } = string.Empty;

        public required string Biography { get; set; }

        public required Uri ProfilePhoto { get; set; }

        public DateTime? LastConnectionDate { get; set; }

        public List<string> ConnectionIds { get; set; } = [];

        public required DateTime BirthDate { get; set; }

        public required UserSettings Settings { get; set; }

        public required DateTime CreatedDate { get; set; }
    }
}