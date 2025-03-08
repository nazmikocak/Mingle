using Firebase.Auth;
using Firebase.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Mingle.API.Hubs;
using Mingle.Services.Abstract;
using Mingle.Services.Exceptions;
using Mingle.Shared.DTOs.Request;

namespace Mingle.API.Controllers
{
    /// <summary>
    /// Kullanıcı ile ilgili işlemleri gerçekleştiren API denetleyicisi.
    /// Kullanıcı bilgileri, profil fotoğrafı, adı, telefon numarası gibi veriler üzerinde güncellemeler yapılabilir.
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public sealed class UserController : BaseController
    {
        private readonly IHubContext<NotificationHub> _notificationHubContext;
        private readonly IUserService _userService;



        /// <summary>
        /// UserController sınıfının yapıcı metodudur.
        /// Gerekli servisleri alarak controller'ı başlatır.
        /// </summary>
        /// <param name="notificationHubContext">Bildirim hub'ı için <see cref="IHubContext{NotificationHub}"/> nesnesi.</param>
        /// <param name="userService">Kullanıcı işlemleri için <see cref="IUserService"/> nesnesi.</param>
        public UserController(IHubContext<NotificationHub> notificationHubContext, IUserService userService)
        {
            _notificationHubContext = notificationHubContext;
            _userService = userService;
        }



        /// <summary>
        /// Kullanıcı bilgilerini getirir.
        /// Kullanıcının bilgileri, kullanıcı kimliği üzerinden alınır ve geri döndürülür.
        /// </summary>
        /// <returns>Bir <see cref="IActionResult"/> döndürür, başarılı olduğunda kullanıcı bilgilerini içerir.</returns>
        /// <exception cref="NotFoundException"> Eğer kullanıcı bulunamazsa fırlatılır.</exception>
        /// <exception cref="FirebaseException"> Firebase ile ilgili bir hata oluşursa fırlatılır.</exception>
        /// <exception cref="Exception">Beklenmedik bir hata durumunda fırlatılır.</exception>
        [HttpGet]
        public async Task<IActionResult> UserInfo()
        {
            try
            {
                return Ok(await _userService.GetUserInfoAsync(UserId));
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (FirebaseException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Firebase ile ilgili bir hata oluştu!", errorDetails = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Beklenmedik bir hata oluştu!", errorDetails = ex.Message });
            }
        }



        /// <summary>
        /// Kullanıcının profil fotoğrafını kaldırır.
        /// Profil fotoğrafı kaldırıldığında, ilgili değişiklikler tüm istemcilere bildirilir.
        /// </summary>
        /// <returns>Bir <see cref="IActionResult"/> döndürür, başarılı olduğunda profil fotoğrafının kaldırıldığını bildirir.</returns>
        /// <exception cref="FirebaseException">Firebase ile ilgili bir hata oluşursa fırlatılır.</exception>
        /// <exception cref="Exception">Beklenmedik bir hata durumunda fırlatılır.</exception>
        [HttpDelete]
        public async Task<IActionResult> ProfilePhoto()
        {
            try
            {
                var profilePhoto = await _userService.RemoveProfilePhotoAsync(UserId);
                await _notificationHubContext.Clients.All.SendAsync("ReceiveRecipientProfiles", new Dictionary<string, Dictionary<string, object>> { { UserId, new Dictionary<string, object> { { "profilePhoto", profilePhoto } } } });

                return Ok(new { message = "Profil fotoğrafı kaldırıldı.", profilePhoto });
            }
            catch (FirebaseException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Firebase ile ilgili bir hata oluştu!", errorDetails = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Beklenmedik bir hata oluştu!", errorDetails = ex.Message });
            }
        }



        /// <summary>
        /// Kullanıcının profil fotoğrafını günceller.
        /// Yeni profil fotoğrafı başarıyla güncellenir ve değişiklikler tüm istemcilere bildirilir.
        /// </summary>
        /// <param name="dto">Profil fotoğrafı güncelleme bilgilerini içeren <see cref="UpdateProfilePhoto"/> nesnesi.</param>
        /// <returns>Bir <see cref="IActionResult"/> döndürür, başarılı olduğunda yeni profil fotoğrafını içerir.</returns>
        /// <exception cref="BadRequestException">Eğer gönderilen veri geçerli değilse fırlatılır.</exception>
        /// <exception cref="FirebaseException">Firebase ile ilgili bir hata oluşursa fırlatılır.</exception>
        /// <exception cref="Exception">Beklenmedik bir hata durumunda fırlatılır.</exception>
        [HttpPatch]
        public async Task<IActionResult> ProfilePhoto([FromBody] UpdateProfilePhoto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var profilePhoto = await _userService.UpdateProfilePhotoAsync(UserId, dto);
                await _notificationHubContext.Clients.All.SendAsync("ReceiveRecipientProfiles", new Dictionary<string, Dictionary<string, object>> { { UserId, new Dictionary<string, object> { { "profilePhoto", profilePhoto } } } });

                return Ok(new { message = "Profil fotoğrafı güncellendi.", profilePhoto });
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (FirebaseException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Firebase ile ilgili bir hata oluştu!", errorDetails = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Beklenmedik bir hata oluştu!", errorDetails = ex.Message });
            }
        }



        /// <summary>
        /// Kullanıcının adı (displayName) güncellenir.
        /// Yeni kullanıcı adı başarılı şekilde güncellenir ve değişiklik tüm istemcilere bildirilir.
        /// </summary>
        /// <param name="dto">Kullanıcı adı güncelleme bilgilerini içeren <see cref="UpdateDisplayName"/> nesnesi.</param>
        /// <returns>Bir <see cref="IActionResult"/> döndürür, başarılı olduğunda yeni kullanıcı adı bilgisi içerir.</returns>
        /// <exception cref="FirebaseException">Firebase ile ilgili bir hata oluşursa fırlatılır.</exception>
        /// <exception cref="Exception">Beklenmedik bir hata durumunda fırlatılır.</exception>
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
                await _notificationHubContext.Clients.All.SendAsync("ReceiveRecipientProfiles", new Dictionary<string, Dictionary<string, object>> { { UserId, new Dictionary<string, object> { { "displayName", dto.DisplayName } } } });

                return Ok(new { message = "Kullanıcı adı güncellendi." });
            }
            catch (FirebaseException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Firebase ile ilgili bir hata oluştu!", errorDetails = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Beklenmedik bir hata oluştu!", errorDetails = ex.Message });
            }
        }



        /// <summary>
        /// Kullanıcının telefon numarasını günceller.
        /// Yeni telefon numarası başarılı şekilde güncellenir ve değişiklik tüm istemcilere bildirilir.
        /// </summary>
        /// <param name="dto">Telefon numarası güncelleme bilgilerini içeren <see cref="UpdatePhoneNumber"/> nesnesi.</param>
        /// <returns>Bir <see cref="IActionResult"/> döndürür, başarılı olduğunda yeni telefon numarası bilgisi içerir.</returns>
        /// <exception cref="FirebaseException">Firebase ile ilgili bir hata oluşursa fırlatılır.</exception>
        /// <exception cref="Exception">Beklenmedik bir hata durumunda fırlatılır.</exception>
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
                await _notificationHubContext.Clients.All.SendAsync("ReceiveRecipientProfiles", new Dictionary<string, Dictionary<string, object>> { { UserId, new Dictionary<string, object> { { "phoneNumber", dto.PhoneNumber } } } });

                return Ok(new { message = "Telefon numrası güncellendi." });
            }
            catch (FirebaseException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Firebase ile ilgili bir hata oluştu.", errorDetails = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Beklenmedik bir hata oluştu.", errorDetails = ex.Message });
            }
        }



        /// <summary>
        /// Kullanıcının biyografisini günceller.
        /// Yeni biyografi başarıyla güncellenir ve değişiklik tüm istemcilere bildirilir.
        /// </summary>
        /// <param name="dto">Biyografi güncelleme bilgilerini içeren <see cref="UpdateBiography"/> nesnesi.</param>
        /// <returns>Bir <see cref="IActionResult"/> döndürür, başarılı olduğunda yeni biyografi bilgisi içerir.</returns>
        /// <exception cref="FirebaseException">Firebase ile ilgili bir hata oluşursa fırlatılır.</exception>
        /// <exception cref="Exception">Beklenmedik bir hata durumunda fırlatılır.</exception>
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
                await _notificationHubContext.Clients.All.SendAsync("ReceiveRecipientProfiles", new Dictionary<string, Dictionary<string, object>> { { UserId, new Dictionary<string, object> { { "biography", dto.Biography } } } });

                return Ok(new { message = "Biyografi güncellendi." });
            }
            catch (FirebaseException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Firebase ile ilgili bir hata oluştu!", errorDetails = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Beklenmedik bir hata oluştu!", errorDetails = ex.Message });
            }
        }



        /// <summary>
        /// Kullanıcının şifresini değiştirir.
        /// Yeni şifre başarıyla güncellenir.
        /// </summary>
        /// <param name="dto">Şifre değiştirme bilgilerini içeren <see cref="ChangePassword"/> nesnesi.</param>
        /// <returns>Bir <see cref="IActionResult"/> döndürür, başarılı olduğunda işlem mesajını içerir.</returns>
        /// <exception cref="FirebaseAuthHttpException">Hatalı giriş bilgisi, çok fazla deneme veya kullanıcı engellenmişse ilgili hata mesajları döndürülür.</exception>
        /// <exception cref="FirebaseException">Firebase ile ilgili bir hata oluşursa fırlatılır.</exception>
        /// <exception cref="Exception">Beklenmedik bir hata durumunda fırlatılır.</exception>
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
                    return Unauthorized(new { message = "Mevcut şifreniz hatalı.", errorDetails = ex.Message });
                }
                return ex.Reason switch
                {
                    AuthErrorReason.TooManyAttemptsTryLater => StatusCode(StatusCodes.Status403Forbidden, new { message = "Çok fazla giriş denemesi yapıldı. Lütfen daha sonra tekrar deneyiniz.", errorDetails = ex.Message }),
                    AuthErrorReason.OperationNotAllowed => StatusCode(StatusCodes.Status403Forbidden, new { message = "Bu işlem şu anda geçerli değil.", errorDetails = ex.Message }),
                    AuthErrorReason.UserDisabled => StatusCode(StatusCodes.Status403Forbidden, new { message = "Hesabınız devre dışı bırakılmıştır.", errorDetails = ex.Message }),
                    _ => StatusCode(500, new { message = $"Firebase ile ilgili bir hata oluştu!", errorDetails = ex.Message })
                };
            }
            catch (FirebaseException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Firebase ile ilgili bir hata oluştu!", errorDetails = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Beklenmedik bir hata oluştu!", errorDetails = ex.Message });
            }
        }



        /// <summary>
        /// Kullanıcının tema tercihini günceller.
        /// Yeni tema başarıyla güncellenir.
        /// </summary>
        /// <param name="dto">Tema değiştirme bilgilerini içeren <see cref="ChangeTheme"/> nesnesi.</param>
        /// <returns>Bir <see cref="IActionResult"/> döndürür, başarılı olduğunda işlem mesajını içerir.</returns>
        /// <exception cref="FirebaseException">Firebase ile ilgili bir hata oluşursa fırlatılır.</exception>
        /// <exception cref="Exception">Beklenmedik bir hata durumunda fırlatılır.</exception>
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
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Firebase ile ilgili bir hata oluştu!", errorDetails = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Beklenmedik bir hata oluştu!", errorDetails = ex.Message });
            }
        }



        /// <summary>
        /// Kullanıcının sohbet arka planını günceller.
        /// Yeni sohbet arka planı başarıyla güncellenir.
        /// </summary>
        /// <param name="dto">Sohbet arka planı değiştirme bilgilerini içeren <see cref="ChangeChatBackground"/> nesnesi.</param>
        /// <returns>Bir <see cref="IActionResult"/> döndürür, başarılı olduğunda işlem mesajını içerir.</returns>
        /// <exception cref="FirebaseException">Firebase ile ilgili bir hata oluşursa fırlatılır.</exception>
        /// <exception cref="Exception">Beklenmedik bir hata durumunda fırlatılır.</exception>
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
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Firebase ile ilgili bir hata oluştu!", errorDetails = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Beklenmedik bir hata oluştu!", errorDetails = ex.Message });
            }
        }
    }
}