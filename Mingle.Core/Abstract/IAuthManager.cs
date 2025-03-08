using Mingle.Shared.DTOs.Request;

namespace Mingle.Core.Abstract
{
    public interface IAuthManager
    {
        (bool IsValid, string ErrorMessage) ValidateGoogleProvider(SignInProvider dto);

        (bool IsValid, string ErrorMessage) ValidateFacebookProvider(SignInProvider dto);
    }
}