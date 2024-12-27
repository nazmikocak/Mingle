using Firebase.Auth;
using Firebase.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mingle.DataAccess.Abstract;
using Mingle.Services.Abstract;
using Mingle.Services.DTOs.Request;
using Mingle.Services.Exceptions;

namespace Mingle.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public sealed class UserController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IChatService _chatService;

        public UserController(IUserService userService, IChatService chatService)
        {
            _userService = userService;
            _chatService = chatService;
        }



        // GET: Users
        [HttpGet]
        public async Task<IActionResult> Users([FromQuery] SearchedUsers dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                return Ok(await _userService.SearchUsersAsync(UserId, dto));
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (FirebaseException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Firebase ile ilgili bir hata oluştu: {ex.Message}" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Beklenmedik bir hata oluştu: {ex.Message}" });
            }
        }



        // GET: UserProfile
        [HttpGet]
        public async Task<IActionResult> UserProfile()
        {
            try
            {
                return Ok(await _userService.GetUserProfileAsync(UserId));
            }
            catch (FirebaseException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Firebase ile ilgili bir hata oluştu: {ex.Message}" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Beklenmedik bir hata oluştu: {ex.Message}" });
            }
        }



        // DELETE: ProfilePhoto
        [HttpDelete]
        public async Task<IActionResult> ProfilePhoto()
        {
            try
            {
                return Ok(new { message = "Profil fotoğrafı kaldırıldı.", profilePhoto = await _userService.RemoveProfilePhotoAsync(UserId) });
            }
            catch (FirebaseException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Firebase ile ilgili bir hata oluştu: {ex.Message}" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Beklenmedik bir hata oluştu: {ex.Message}" });
            }
        }



        // PATCH: ProfilePhoto
        [HttpPatch]
        public async Task<IActionResult> ProfilePhoto([FromForm] UpdateProfilePhoto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                return Ok(new { message = "Profil fotoğrafı güncellendi.", profilePhoto = await _userService.UpdateProfilePhotoAsync(UserId, dto) });
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (FirebaseException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Firebase ile ilgili bir hata oluştu: {ex.Message}" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Beklenmedik bir hata oluştu: {ex.Message}" });
            }
        }



        // PATCH: DisplayName
        [HttpPatch]
        public async Task<IActionResult> DisplayName([FromBody] UpdateDisplayName dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                await _userService.UpdateDisplayNameAsync(UserId, dto);
                return Ok(new { message = "Kullanıcı adı güncellendi." });
            }
            catch (FirebaseException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Firebase ile ilgili bir hata oluştu: {ex.Message}" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Beklenmedik bir hata oluştu: {ex.Message}" });
            }
        }



        // PATCH: PhoneNumber
        [HttpPatch]
        public async Task<IActionResult> PhoneNumber([FromBody] UpdatePhoneNumber dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                await _userService.UpdatePhoneNumberAsync(UserId, dto);
                return Ok(new { message = "Telefon numrası güncellendi." });
            }
            catch (FirebaseException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Firebase ile ilgili bir hata oluştu: {ex.Message}" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Beklenmedik bir hata oluştu: {ex.Message}" });
            }
        }



        // PATCH: Biography
        [HttpPatch]
        public async Task<IActionResult> Biography([FromBody] UpdateBiography dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                await _userService.UpdateBiographyAsync(UserId, dto);
                return Ok(new { message = "Biyografi güncellendi." });
            }
            catch (FirebaseException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Firebase ile ilgili bir hata oluştu: {ex.Message}" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Beklenmedik bir hata oluştu: {ex.Message}" });
            }
        }



        // PATCH: Password
        [HttpPatch]
        public async Task<IActionResult> Password([FromBody] ChangePassword dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                await _userService.ChangePasswordAsync(UserId, dto);
                return Ok(new { message = "Şifre değiştirildi." });
            }
            catch (FirebaseAuthHttpException ex)
            {
                if (ex.Message.Contains("INVALID_LOGIN_CREDENTIALS"))
                {
                    return Unauthorized(new { message = "Mevcut şifreniz hatalı." });
                }
                return ex.Reason switch
                {
                    AuthErrorReason.TooManyAttemptsTryLater => StatusCode(StatusCodes.Status403Forbidden, new { message = "Çok fazla giriş denemesi yapıldı. Lütfen daha sonra tekrar deneyiniz." }),
                    AuthErrorReason.OperationNotAllowed => StatusCode(StatusCodes.Status403Forbidden, new { message = "Bu işlem şu anda geçerli değil." }),
                    AuthErrorReason.UserDisabled => StatusCode(StatusCodes.Status403Forbidden, new { message = "Hesabınız devre dışı bırakılmıştır." }),
                    _ => StatusCode(500, new { message = $"Firebase ile ilgili bir hata oluştu: {ex.Message}" })
                };
            }
            catch (FirebaseException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Firebase ile ilgili bir hata oluştu: {ex.Message}" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Beklenmedik bir hata oluştu: {ex.Message}" });
            }
        }



        // PATCH: Theme
        [HttpPatch]
        public async Task<IActionResult> Theme([FromBody] ChangeTheme dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                await _userService.ChangeThemeAsync(UserId, dto);
                return Ok(new { message = "Tema güncellendi." });
            }
            catch (FirebaseException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Firebase ile ilgili bir hata oluştu: {ex.Message}" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Beklenmedik bir hata oluştu: {ex.Message}" });
            }
        }



        // PATCH: ChatBackground
        [HttpPatch]
        public async Task<IActionResult> ChatBackground([FromBody] ChangeChatBackground dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                await _userService.ChangeChatBackgroundAsync(UserId, dto);
                return Ok(new { message = "Sohbet arka planı güncellendi." });
            }
            catch (FirebaseException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Firebase ile ilgili bir hata oluştu: {ex.Message}" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Beklenmedik bir hata oluştu: {ex.Message}" });
            }
        }


        [HttpGet]
        public async Task<IActionResult> TestFirebase()
        {
            return Ok(await _chatService.GetChatsAsync(UserId));
        }
    }
}