using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Mingle.API.Controllers
{
    /// <summary>
    /// API isteklerinde ortak işlevsellikleri yöneten temel denetleyici sınıfıdır.
    /// Tüm denetleyiciler bu sınıftan türemekte olup, kullanıcı kimliğine erişim sağlayabilir.
    /// </summary>
    [Authorize]
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        /// <summary>
        /// Geçerli kullanıcının kimliğini (UserId) döndürür.
        /// Kullanıcının kimliği, JWT içindeki <see cref="ClaimTypes.NameIdentifier"/> değerinden alınır.
        /// </summary>
        /// <returns>Geçerli kullanıcının benzersiz kimliği.</returns>
        /// <exception cref="NullReferenceException">
        /// Eğer kullanıcı kimliği bulunamazsa veya bir null değer ile karşılaşılırsa fırlatılır.
        /// </exception>
        protected string UserId
        {
            get
            {
                var identity = HttpContext?.User?.Identity as ClaimsIdentity;
                return identity?
                    .FindFirst(ClaimTypes.NameIdentifier)?
                    .Value!;
            }
        }
    }
}