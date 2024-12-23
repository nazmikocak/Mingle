/*

using Firebase.Database;
using Microsoft.AspNetCore.Mvc;
using Mingle.Services.Abstract;
using Mingle.Services.Exceptions;

namespace Mingle.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public sealed class IndividualChatController : BaseController
    {
        private readonly IChatService _individualChatService;



        public IndividualChatController(IChatService ındividualChatService)
        {
            _individualChatService = ındividualChatService;
        }



        // POST: CreateChat
        [HttpPost("{recipientId}")]
        public async Task<IActionResult> CreateChat([FromRoute(Name = "recipientId")] string recipientId)
        {
            try
            {
                return Ok(await _individualChatService.CreateIndividualChatAsync(UserId, recipientId));
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
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



        // DELETE: ClearChat
        [HttpDelete("{chatId:guid}")]
        public async Task<IActionResult> ClearChat([FromRoute(Name = "chatId")] string chatId)
        {
            try
            {
                await _individualChatService.ClearIndividualChatAsync(UserId, chatId);
                return Ok(new { message = "Sohbet temizlendi." });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ForbiddenException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
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



        // PATCH: ArchiveChat
        [HttpPatch("{chatId:guid}")]
        public async Task<IActionResult> ArchiveChat([FromRoute(Name = "chatId")] string chatId)
        {
            try
            {
                await _individualChatService.ArchiveIndividualChatAsync(UserId, chatId);
                return Ok(new { message = "Sohbet arşivlendi." });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ForbiddenException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
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



        // PATCH: UnarchiveChat
        [HttpPatch("{chatId:guid}")]
        public async Task<IActionResult> UnarchiveChat([FromRoute(Name = "chatId")] string chatId)
        {
            try
            {
                await _individualChatService.UnarchiveIndividualChatAsync(UserId, chatId);
                return Ok(new { message = "Sohbet arşivden çıkarıldı." });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ForbiddenException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
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



        // GET: RecipientProfile
        [HttpGet("{chatId:guid}")]
        public async Task<IActionResult> RecipientProfile([FromRoute(Name = "chatId")] string chatId)
        {
            try
            {
                return Ok(await _individualChatService.RecipientProfileAsync(UserId, chatId));
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ForbiddenException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
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



        // GET: Messages
        [HttpGet("{chatId:guid}")]
        public async Task<IActionResult> Messages([FromRoute(Name = "chatId")] string chatId)
        {
            try
            {
                return Ok(await _individualChatService.GetMessagesAsync(UserId, chatId));
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ForbiddenException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
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




        // WebSocket //
        // DELETE: Message
        [HttpDelete("{chatId}/{messageId}")]
        public async Task<IActionResult> Message([FromRoute(Name = "chatId")] string chatId, [FromRoute(Name = "messageId")] string messageId, [FromQuery(Name = "deletionType")] byte deletionType)
        {
            try
            {
                await _individualChatService.DeleteMessageAsync(UserId, chatId, messageId, deletionType);
                return Ok(new { message = "Mesaj silindi." });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ForbiddenException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
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

*/