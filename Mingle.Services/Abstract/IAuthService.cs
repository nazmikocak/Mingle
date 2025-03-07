using Mingle.Shared.DTOs.Request;

namespace Mingle.Services.Abstract
{
    public interface IAuthService
    {
        Task SignUpAsync(SignUp dto);

        Task<string> SignInEmailAsync(SignInEmail dto);

        Task<string> SignInGoogleAsync(SignInGoogle dto);

        Task<string> SignInFacebookAsync(string accessToken);

        Task ResetPasswordAsync(string email);
    }
}