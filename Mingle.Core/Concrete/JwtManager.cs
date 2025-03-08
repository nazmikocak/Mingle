using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Mingle.Core.Abstract;
using System.Security.Claims;
using System.Text;

namespace Mingle.Core.Concrete
{
    /// <summary>
    /// JSON Web Token (JWT) oluşturma işlemlerini yöneten sınıf.
    /// </summary>
    /// <remarks>
    /// Bu sınıf, JWT oluşturma işlemini gerçekleştirmek için gerekli yapılandırmaları
    /// alır ve <see cref="GenerateToken"/> metoduyla token üretir.
    /// </remarks>
    public sealed class JwtManager : IJwtManager
    {
        private readonly string? _secret;
        private readonly string? _issuer;
        private readonly string? _audience;
        private readonly byte _expiryInDays;



        /// <summary>
        /// <see cref="JwtManager"/> sınıfının yapıcı metodudur.
        /// JWT yapılandırma ayarlarını almak için <see cref="IConfiguration"/> nesnesi kullanır.
        /// </summary>
        /// <param name="configuration">JWT ayarlarını içeren <see cref="IConfiguration"/> nesnesi.</param>
        public JwtManager(IConfiguration configuration)
        {
            _secret = configuration["JwtSettings:Secret"];
            _issuer = configuration["JwtSettings:Issuer"];
            _audience = configuration["JwtSettings:Audience"];
            _expiryInDays = byte.Parse(configuration["JwtSettings:ExpiryInDays"]!);
        }



        /// <summary>
        /// Verilen kullanıcı kimliği için bir JSON Web Token (JWT) oluşturur.
        /// </summary>
        /// <param name="userId">JWT'yi oluşturmak için kullanılacak kullanıcı kimliği.</param>
        /// <returns>Oluşturulan JWT'yi temsil eden bir dize döner.</returns>
        /// <exception cref="ArgumentNullException">Geçersiz veya eksik yapılandırma verisi durumunda fırlatılır.</exception>
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