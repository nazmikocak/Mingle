using Microsoft.IdentityModel.Tokens;

namespace Mingle.Core.Abstract
{
    public interface IJwtManager
    {
        string GenerateToken(string userId);
    }
}