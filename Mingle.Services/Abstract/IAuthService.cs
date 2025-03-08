using Mingle.Shared.DTOs.Request;

namespace Mingle.Services.Abstract
{
    public interface IAuthService
    {
        Task SignUpAsync(SignUp dto);

        Task<string> SignInEmailAsync(SignInEmail dto);

        Task<string> SignInGoogleAsync(SignInProvider dto);

        Task<string> SignInFacebookAsync(SignInProvider dto);

        Task ResetPasswordAsync(string email);
    }
}