namespace Mingle.Shared.DTOs.Response
{
    /// <summary>
    /// Arama sonucunda bulunan kullanıcıların temel bilgilerini içeren veri transfer nesnesi (DTO).
    /// </summary>
    public sealed record FoundUsers
    {
        public required string DisplayName { get; init; }

        public required string Email { get; init; }

        public required Uri ProfilePhoto { get; init; }
    }
}