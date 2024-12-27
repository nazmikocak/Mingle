namespace Mingle.Entities.Models
{
    public class ConnectionSettings
    {
        public DateTime? LastConnectionDate { get; set; }

        public List<string> ConnectionIds { get; set; } = [];
    }
}