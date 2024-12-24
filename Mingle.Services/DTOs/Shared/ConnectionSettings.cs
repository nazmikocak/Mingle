namespace Mingle.Services.DTOs.Shared
{
    public sealed record ConnectionSettings
    {
        public DateTime? LastConnectionDate { get; set; }

        public List<string> ConnectionIds { get; set; } = [];
    }
}