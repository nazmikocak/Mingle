using Mingle.Shared.DTOs.Request;

namespace Mingle.Core.Abstract
{
    public interface IAuthManager
    {
        (bool IsValid, string ErrorMessage) ValidateGoogle(SignInGoogle dto);
    }
}