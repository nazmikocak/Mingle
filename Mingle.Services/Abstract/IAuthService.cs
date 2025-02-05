using Mingle.Services.DTOs.Request;

namespace Mingle.Services.Abstract
{
    public interface IAuthService
    {
        Task SignUpAsync(SignUp dto);

        Task<string> SignInEmailAsync(SignIn dto);

        Task<string> SignInGoogleAsync(string accessToken);

        Task<string> SignInFacebookAsync(string accessToken);

        Task ResetPasswordAsync(string email);
    }
}