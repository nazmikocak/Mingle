using Firebase.Auth;
using Microsoft.AspNetCore.Mvc;
using Mingle.Services.Abstract;
using Mingle.Shared.DTOs.Request;
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



        // POST: SignInEmail
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



        // POST: SignInGoogle
        [HttpPost]
        public async Task<IActionResult> SignInGoogle([FromBody] SignInGoogle dto)
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



        // POST: SignInFacebook
        [HttpPost]
        public async Task<IActionResult> SignInFacebook([FromBody] string accessToken)
        {
            try
            {
                return Ok(new { token = await _authService.SignInFacebookAsync(accessToken) });
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



        // POST: SignOut
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



        // POST: Password
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