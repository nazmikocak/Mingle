using Firebase.Auth;
using Microsoft.AspNetCore.Mvc;
using Mingle.Services.Abstract;
using Mingle.Services.Exceptions;
using Mingle.Shared.DTOs.Request;

namespace Mingle.API.Controllers
{
    /// <summary>
    /// Kullanıcı kimlik doğrulama işlemlerini yöneten API controller sınıfıdır.
    /// Kullanıcı kaydı, oturum açma, sosyal medya ile giriş işlemlerini yönetir.
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public sealed class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;



        /// <summary>
        /// <see cref="AuthController"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        /// <param name="authService">Kullanıcı kimlik doğrulama işlemleri için <see cref="IAuthService"/> bağımlılığı.</param>
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }



        /// <summary>
        /// Kullanıcı kaydını gerçekleştirir.
        /// Geçersiz girişler ve hata durumlarında uygun cevaplar döner.
        /// </summary>
        /// <param name="dto">Kayıt için gerekli olan kullanıcı bilgilerini içeren <see cref="SignUp"/> veri transfer objesi.</param>
        /// <returns>Bir <see cref="IActionResult"/> döner.</returns>
        /// <exception cref="BadRequestException">Geçersiz kayıt işlemi durumunda fırlatılır.</exception>
        /// <exception cref="FirebaseAuthHttpException">Firebase ile ilgili bir hata oluştuğunda fırlatılır.</exception>
        /// <exception cref="Exception">Beklenmedik bir hata oluşursa fırlatılır.</exception>
        [HttpPost]
        public async Task<IActionResult> SignUp([FromBody] SignUp dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                await _authService.SignUpAsync(dto);
                return Ok(new { message = "Kullanıcı başarıyla kaydedildi." });
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (FirebaseAuthHttpException ex)
            {
                return ex.Reason switch
                {
                    AuthErrorReason.EmailExists => Conflict(new { message = "Bu e-posta adresi zaten kullanılıyor.", errorDetails = ex.Message }),
                    AuthErrorReason.OperationNotAllowed => StatusCode(StatusCodes.Status403Forbidden, new { message = "Bu işlem şu anda geçerli değil.", errorDetails = ex.Message }),
                    AuthErrorReason.TooManyAttemptsTryLater => StatusCode(StatusCodes.Status403Forbidden, new { message = "Çok fazla kayıt denemesi yapıldı. Lütfen daha sonra tekrar deneyin.", errorDetails = ex.Message }),
                    _ => StatusCode(500, new { message = $"Firebase ile ilgili bir hata oluştu!", errorDetails = ex.Message })
                };
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Beklenmedik bir hata oluştu!", errorDetails = ex.Message });
            }
        }



        /// <summary>
        /// E-posta ve şifre ile giriş işlemi gerçekleştirir.
        /// Geçersiz kimlik bilgileri ve hata durumlarında uygun cevaplar döner.
        /// </summary>
        /// <param name="dto">Giriş için gerekli olan e-posta ve şifre bilgilerini içeren <see cref="SignInEmail"/> veri transfer objesi.</param>
        /// <returns>Bir <see cref="IActionResult"/> döner.</returns>
        /// <exception cref="FirebaseAuthHttpException">Firebase ile ilgili bir hata oluştuğunda fırlatılır.</exception>
        /// <exception cref="Exception">Beklenmedik bir hata oluşursa fırlatılır.</exception>
        [HttpPost]
        public async Task<IActionResult> SignInEmail([FromBody] SignInEmail dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                return Ok(new { token = await _authService.SignInEmailAsync(dto) });
            }
            catch (FirebaseAuthHttpException ex)
            {
                if (ex.Message.Contains("INVALID_LOGIN_CREDENTIALS"))
                {
                    return Unauthorized(new { message = "E-posta ya da şifre hatalı.", errorDetails = ex.Message });
                }
                return ex.Reason switch
                {
                    AuthErrorReason.TooManyAttemptsTryLater => StatusCode(StatusCodes.Status403Forbidden, new { message = "Çok fazla giriş denemesi yapıldı. Lütfen daha sonra tekrar deneyiniz.", errorDetails = ex.Message }),
                    AuthErrorReason.OperationNotAllowed => StatusCode(StatusCodes.Status403Forbidden, new { message = "Bu işlem şu anda geçerli değil.", errorDetails = ex.Message }),
                    AuthErrorReason.UserDisabled => StatusCode(StatusCodes.Status403Forbidden, new { message = "Hesabınız devre dışı bırakılmıştır.", errorDetails = ex.Message }),
                    _ => StatusCode(500, new { message = $"Firebase ile ilgili bir hata oluştu!", errorDetails = ex.Message })
                };
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Beklenmedik bir hata oluştu!", errorDetails = ex.Message });
            }
        }



        /// <summary>
        /// Google ile giriş işlemi gerçekleştirir.
        /// Google girişinin geçersiz olması ve diğer hata durumlarında uygun cevaplar döner.
        /// </summary>
        /// <param name="dto">Google ile giriş için gerekli olan bilgileri içeren <see cref="SignInGoogle"/> veri transfer objesi.</param>
        /// <returns>Bir <see cref="IActionResult"/> döner.</returns>
        /// <exception cref="BadRequestException">Geçersiz giriş işlemi durumunda fırlatılır.</exception>
        /// <exception cref="FirebaseAuthHttpException">Firebase ile ilgili bir hata oluştuğunda fırlatılır.</exception>
        /// <exception cref="Exception">Beklenmedik bir hata oluşursa fırlatılır.</exception>
        [HttpPost]
        public async Task<IActionResult> SignInGoogle([FromBody] SignInProvider dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                return Ok(new { token = await _authService.SignInGoogleAsync(dto) });
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (FirebaseAuthHttpException ex)
            {
                return ex.Reason switch
                {
                    AuthErrorReason.TooManyAttemptsTryLater => StatusCode(StatusCodes.Status403Forbidden, new { message = "Çok fazla giriş denemesi yapıldı. Lütfen daha sonra tekrar deneyiniz.", errorDetails = ex.Message }),
                    AuthErrorReason.OperationNotAllowed => StatusCode(StatusCodes.Status403Forbidden, new { message = "Bu işlem şu anda geçerli değil.", errorDetails = ex.Message }),
                    AuthErrorReason.UserDisabled => StatusCode(StatusCodes.Status403Forbidden, new { message = "Hesabınız devre dışı bırakılmıştır.", errorDetails = ex.Message }),
                    _ => StatusCode(500, new { message = $"Firebase ile ilgili bir hata oluştu!", errorDetails = ex.Message })
                };
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Beklenmedik bir hata oluştu!", errorDetails = ex.Message });
            }
        }



        /// <summary>
        /// Facebook ile giriş işlemi gerçekleştirir.
        /// Facebook girişinin geçersiz olması ve diğer hata durumlarında uygun cevaplar döner.
        /// </summary>
        /// <param name="dto">Facebook ile giriş için gerekli olan bilgileri içeren <see cref="SignInProvider"/> veri transfer objesi.</param>
        /// <returns>Bir <see cref="IActionResult"/> döner.</returns>
        /// <exception cref="BadRequestException">Geçersiz giriş işlemi durumunda fırlatılır.</exception>
        /// <exception cref="FirebaseAuthHttpException">Firebase ile ilgili bir hata oluştuğunda fırlatılır.</exception>
        /// <exception cref="Exception">Beklenmedik bir hata oluşursa fırlatılır.</exception>
        [HttpPost]
        public async Task<IActionResult> SignInFacebook([FromBody] SignInProvider dto)
        {
            try
            {
                return Ok(new { token = await _authService.SignInFacebookAsync(dto) });
            }
            catch (FirebaseAuthHttpException ex)
            {
                return ex.Reason switch
                {
                    AuthErrorReason.TooManyAttemptsTryLater => StatusCode(StatusCodes.Status403Forbidden, new { message = "Çok fazla giriş denemesi yapıldı. Lütfen daha sonra tekrar deneyiniz.", errorDetails = ex.Message }),
                    AuthErrorReason.OperationNotAllowed => StatusCode(StatusCodes.Status403Forbidden, new { message = "Bu işlem şu anda geçerli değil.", errorDetails = ex.Message }),
                    AuthErrorReason.UserDisabled => StatusCode(StatusCodes.Status403Forbidden, new { message = "Hesabınız devre dışı bırakılmıştır.", errorDetails = ex.Message }),
                    _ => StatusCode(500, new { message = $"Firebase ile ilgili bir hata oluştu!", errorDetails = ex.Message })
                };
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Beklenmedik bir hata oluştu!", errorDetails = ex.Message });
            }
        }



        /// <summary>
        /// Kullanıcıyı oturumdan çıkarır.
        /// Firebase ile ilgili hata oluşması durumunda uygun cevap döner.
        /// </summary>
        /// <returns>Bir <see cref="IActionResult"/> döner.</returns>
        /// <exception cref="FirebaseAuthHttpException">Firebase ile ilgili bir hata oluştuğunda fırlatılır.</exception>
        /// <exception cref="Exception">Beklenmedik bir hata oluşursa fırlatılır.</exception>
        [HttpPost]
        public async Task<IActionResult> SignOut()
        {
            try
            {
                return Ok(new { message = "Oturum kapatıldı." });
            }
            catch (FirebaseAuthHttpException ex)
            {
                return StatusCode(500, new { message = $"Firebase ile ilgili bir hata oluştu!", errorDetails = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Beklenmedik bir hata oluştu!", errorDetails = ex.Message });
            }
        }



        /// <summary>
        /// Şifre sıfırlama bağlantısı gönderir.
        /// Geçersiz e-posta veya hata durumunda uygun cevaplar döner.
        /// </summary>
        /// <param name="email">Şifresi sıfırlanacak kullanıcının e-posta adresi.</param>
        /// <returns>Bir <see cref="IActionResult"/> döner.</returns>
        /// <exception cref="NotFoundException">E-posta adresi bulunamazsa fırlatılır.</exception>
        /// <exception cref="FirebaseAuthHttpException">Firebase ile ilgili bir hata oluştuğunda fırlatılır.</exception>
        /// <exception cref="Exception">Beklenmedik bir hata oluşursa fırlatılır.</exception>
        [HttpPost]
        public async Task<IActionResult> Password([FromBody] string email)
        {
            try
            {
                await _authService.ResetPasswordAsync(email);
                return Ok(new { message = "Şifre sıfırlama bağlantısı gönderildi." });
            }
            catch (NotFoundException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (FirebaseAuthHttpException ex)
            {
                return StatusCode(500, new { message = $"Firebase ile ilgili bir hata oluştu!", errorDetails = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Beklenmedik bir hata oluştu!", errorDetails = ex.Message });
            }
        }
    }
}