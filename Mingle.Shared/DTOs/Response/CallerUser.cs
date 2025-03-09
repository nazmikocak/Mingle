namespace Mingle.Shared.DTOs.Response
{
    /// <summary>
    /// Kullanıcının temel bilgilerini içeren veri transfer nesnesi (DTO).
    /// </summary>
    public sealed record CallerUser
    {
        public required string DisplayName { get; init; }

        public required Uri ProfilePhoto { get; init; }

        public required DateTime LastConnectionDate { get; set; }
    }
}