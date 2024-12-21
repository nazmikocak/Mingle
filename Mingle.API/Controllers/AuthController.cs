using Firebase.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mingle.Services.Abstract;
using Mingle.Services.DTOs;
using Mingle.Services.Exceptions;

namespace Mingle.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public sealed class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // POST: SignUp
        [HttpPost]
        public async Task<IActionResult> SignUp([FromBody] SignUpRequest dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                await _authService.SignUpAsync(dto);
                return Ok(new { Message = "Kullanıcı başarıyla kaydedildi." });
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (FirebaseAuthHttpException ex)
            {
                return ex.Reason switch
                {
                    AuthErrorReason.EmailExists => Conflict(new { message = "Bu e-posta adresi zaten kullanılıyor." }),
                    AuthErrorReason.OperationNotAllowed => StatusCode(StatusCodes.Status403Forbidden, new { message = "Bu işlem şu anda geçerli değil." }),
                    AuthErrorReason.TooManyAttemptsTryLater => StatusCode(StatusCodes.Status403Forbidden, new { message = "Çok fazla kayıt denemesi yapıldı. Lütfen daha sonra tekrar deneyin." }),
                    _ => StatusCode(500, new { message = $"Firebase ile ilgili bir hata oluştu: {ex.Message}" })
                };
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Beklenmedik bir hata oluştu: {ex.Message}" });
            }
        }


        // POST: SignIn
        [HttpPost]
        public async Task<IActionResult> SignIn([FromBody] SignInRequest dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                await _authService.SignInAsync(dto);
                return Ok();
            }
            catch (FirebaseAuthHttpException ex)
            {
                return ex.Reason switch
                {
                    // TODO: Yanlış şifre ve e-posta girildiğinde döndürülen hata mesajları çalışmıyor.

                    AuthErrorReason.WrongPassword => Unauthorized(new { message = "Şifre yanlış" }),
                    AuthErrorReason.UserNotFound => NotFound(new { message = "Kullanıcı bulunamadı." }),
                    AuthErrorReason.UnknownEmailAddress => NotFound(new { message = "E-Mail adresine sahip bir kullanıcı bulunamadı." }),
                    AuthErrorReason.TooManyAttemptsTryLater => StatusCode(StatusCodes.Status403Forbidden, new { message = "Çok fazla giriş denemesi yapıldı. Lütfen daha sonra tekrar deneyiniz." }),
                    AuthErrorReason.OperationNotAllowed => StatusCode(StatusCodes.Status403Forbidden, new { message = "Bu işlem şu anda geçerli değil." }),
                    AuthErrorReason.UserDisabled => StatusCode(StatusCodes.Status403Forbidden, new { message = "Hesabınız devre dışı bırakılmıştır." }),
                    _ => StatusCode(500, new { message = $"Beklenmedik bir hata oluştu: {ex.Message}" })
                };
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Beklenmedik bir hata oluştu: {ex.Message}" });
            }
        }



        // POST: SignOut
        [HttpPost]
        public async Task<IActionResult> SignOut()
        {
            try
            {
                // Client ten JWT silinecek.
                return Ok(new { message = "Oturum kapatıldı." });
            }
            catch (FirebaseAuthHttpException ex)
            {
                return StatusCode(500, new { message = $"Beklenmedik bir hata oluştu: {ex.Message}" });
            }
        }
    }
}