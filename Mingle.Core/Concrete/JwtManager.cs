using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Mingle.Core.Abstract;
using System.Security.Claims;
using System.Text;

namespace Mingle.Core.Concrete
{
    public sealed class JwtManager : IJwtManager
    {
        private readonly string? _secret;
        private readonly string? _issuer;
        private readonly string? _audience;
        private readonly byte _expiryInDays;


        public JwtManager(IConfiguration configuration)
        {
            _secret = configuration["JwtSettings:Secret"];
            _issuer = configuration["JwtSettings:Issuer"];
            _audience = configuration["JwtSettings:Audience"];
            _expiryInDays = byte.Parse(configuration["JwtSettings:ExpiryInDays"]);
        }


        public string GenerateToken(string userId)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(_expiryInDays),
                Issuer = _issuer,
                Audience = _audience,
                SigningCredentials = credentials
            };

            return new JsonWebTokenHandler().CreateToken(tokenDescriptor);
        }
    }
}