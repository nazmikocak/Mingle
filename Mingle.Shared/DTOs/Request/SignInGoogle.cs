namespace Mingle.Shared.DTOs.Request
{
    public class SignInGoogle
    {
        public required string Uid { get; init; }
        public required string Email { get; init; }
        public required bool EmailVerified { get; init; }
        public required string DisplayName { get; init; }
        public required bool IsAnonymous { get; init; }
        public required string PhotoURL { get; init; }
        public required ProviderData[] ProviderData { get; init; }
        public required StsTokenManager StsTokenManager { get; init; }
        public required string CreatedAt { get; init; }
        public required string LastLoginAt { get; init; }
        public required string ApiKey { get; init; }
        public required string AppName { get; init; }
    }

    public sealed record ProviderData
    {
        public required string ProviderId { get; init; }
        public required string Uid { get; init; }
        public required string DisplayName { get; init; }
        public required string Email { get; init; }
        public string? PhoneNumber { get; init; }
        public required string PhotoURL { get; init; }
    }

    public sealed record StsTokenManager
    {
        public required string RefreshToken { get; init; }
        public required string AccessToken { get; init; }
        public required long ExpirationTime { get; init; }
    }
}