using Firebase.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mingle.Services.Abstract;

namespace Mingle.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public sealed class UserController : BaseController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
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
                await _userService.RemoveProfilePhotoAsync(UserId);
                return Ok(new { message = "Profil fotoğrafı kaldırıldı." });
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
    }
}