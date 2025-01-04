using Mingle.Services.DTOs.Request;

namespace Mingle.Services.Abstract
{
    public interface IAuthService
    {
        Task SignUpAsync(SignUp dto);

        Task<string> SignInAsync(SignIn dto);

        Task ResetPasswordAsync(string email);
    }
}