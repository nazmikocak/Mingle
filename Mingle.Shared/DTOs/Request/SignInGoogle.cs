namespace Mingle.Shared.DTOs.Request
{
    public class SignInGoogle
    {
        public required GoogleUser User { get; init; }
        public required string ProviderId { get; init; }
        public required TokenResponse _TokenResponse { get; init; }
        public required string OperationType { get; init; }
    }


    public class GoogleUser
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

    public sealed record TokenResponse
    {
        public required string FederatedId { get; init; }
        public required string ProviderId { get; init; }
        public required string Email { get; init; }
        public required bool EmailVerified { get; init; }
        public required string FirstName { get; init; }
        public required string FullName { get; init; }
        public required string LastName { get; init; }
        public required string PhotoUrl { get; init; }
        public required string LocalId { get; init; }
        public required string DisplayName { get; init; }
        public required string IdToken { get; init; }
        public string? Context { get; init; }
        public required string OauthAccessToken { get; init; }
        public required int OauthExpireIn { get; init; }
        public required string RefreshToken { get; init; }
        public required string ExpiresIn { get; init; }
        public required string OauthIdToken { get; init; }
        public required string RawUserInfo { get; init; }
        public required bool IsNewUser { get; init; }
        public required string Kind { get; init; }
    }
}