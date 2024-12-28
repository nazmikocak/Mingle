//using Firebase.Database;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Mingle.Services.Abstract;
//using Mingle.Services.Exceptions;

//namespace Mingle.API.Controllers
//{
//    [Route("api/[controller]/[action]")]
//    [ApiController]
//    public class ChatController : BaseController
//    {
//        private readonly IChatService _chatService;
//        private readonly IUserService _userService;


//        public ChatController(IChatService chatService, IUserService userService)
//        {
//            _chatService = chatService;
//            _userService = userService;
//        }


//        // POST: CreateChat
//        [HttpPost("{chatType}/{recipientId}")]
//        public async Task<IActionResult> CreateChat([FromRoute(Name = "chatType")] string chatType, [FromRoute(Name = "recipientId")] string recipientId)
//        {
//            try
//            {
//                return Ok(await _chatService.CreateChatAsync(UserId, chatType, recipientId));
//            }
//            catch (NotFoundException ex)
//            {
//                return NotFound(new { message = ex.Message });
//            }
//            catch (BadRequestException ex)
//            {
//                return BadRequest(new { message = ex.Message });
//            }
//            catch (FirebaseException ex)
//            {
//                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Firebase ile ilgili bir hata oluştu: {ex.Message}" });
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Beklenmedik bir hata oluştu: {ex.Message}" });
//            }
//        }



//        // DELETE: ClearChat
//        [HttpDelete("{chatId:guid}")]
//        public async Task<IActionResult> ClearChat([FromRoute(Name = "chatType")] string chatType, [FromRoute(Name = "chatId")] string chatId)
//        {
//            try
//            {
//                await _chatService.ClearChatAsync(UserId, chatType, chatId);
//                return Ok(new { message = "Sohbet temizlendi." });
//            }
//            catch (NotFoundException ex)
//            {
//                return NotFound(new { message = ex.Message });
//            }
//            catch (BadRequestException ex)
//            {
//                return BadRequest(new { message = ex.Message });
//            }
//            catch (ForbiddenException ex)
//            {
//                return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
//            }
//            catch (FirebaseException ex)
//            {
//                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Firebase ile ilgili bir hata oluştu: {ex.Message}" });
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Beklenmedik bir hata oluştu: {ex.Message}" });
//            }
//        }



//        // PATCH: ArchiveChat
//        [HttpPatch("{chatId:guid}")]
//        public async Task<IActionResult> ArchiveChat([FromRoute(Name = "chatId")] string chatId)
//        {
//            try
//            {
//                await _chatService.ArchiveIndividualChatAsync(UserId, chatId);
//                return Ok(new { message = "Sohbet arşivlendi." });
//            }
//            catch (NotFoundException ex)
//            {
//                return NotFound(new { message = ex.Message });
//            }
//            catch (BadRequestException ex)
//            {
//                return BadRequest(new { message = ex.Message });
//            }
//            catch (ForbiddenException ex)
//            {
//                return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
//            }
//            catch (FirebaseException ex)
//            {
//                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Firebase ile ilgili bir hata oluştu: {ex.Message}" });
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Beklenmedik bir hata oluştu: {ex.Message}" });
//            }
//        }



//        // PATCH: UnarchiveChat
//        [HttpPatch("{chatId:guid}")]
//        public async Task<IActionResult> UnarchiveChat([FromRoute(Name = "chatId")] string chatId)
//        {
//            try
//            {
//                await _chatService.UnarchiveIndividualChatAsync(UserId, chatId);
//                return Ok(new { message = "Sohbet arşivden çıkarıldı." });
//            }
//            catch (NotFoundException ex)
//            {
//                return NotFound(new { message = ex.Message });
//            }
//            catch (BadRequestException ex)
//            {
//                return BadRequest(new { message = ex.Message });
//            }
//            catch (ForbiddenException ex)
//            {
//                return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
//            }
//            catch (FirebaseException ex)
//            {
//                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Firebase ile ilgili bir hata oluştu: {ex.Message}" });
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Beklenmedik bir hata oluştu: {ex.Message}" });
//            }
//        }



//        // GET: RecipientProfile
//        [HttpGet("{chatId:guid}")]
//        public async Task<IActionResult> RecipientProfile([FromRoute(Name = "chatId")] string chatId)
//        {
//            try
//            {
//                string recipientId = await _chatService.GetChatRecipientIdAsync(UserId, "Individual", chatId);
//                return Ok(await _userService.GetRecipientProfileAsync(recipientId));

//            }
//            catch (NotFoundException ex)
//            {
//                return NotFound(new { message = ex.Message });
//            }
//            catch (BadRequestException ex)
//            {
//                return BadRequest(new { message = ex.Message });
//            }
//            catch (ForbiddenException ex)
//            {
//                return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
//            }
//            catch (FirebaseException ex)
//            {
//                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Firebase ile ilgili bir hata oluştu: {ex.Message}" });
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Beklenmedik bir hata oluştu: {ex.Message}" });
//            }
//        }
//    }
//}