using Firebase.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Mingle.API.Hubs;
using Mingle.Services.Abstract;
using Mingle.Shared.DTOs.Request;
using Mingle.Services.Exceptions;

namespace Mingle.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public sealed class GroupController : BaseController
    {
        private readonly IHubContext<NotificationHub> _notificationHubContext;
        private readonly IGroupService _groupService;
        private readonly IChatService _chatService;



        public GroupController(IHubContext<NotificationHub> notificationHubContext, IGroupService groupService, IChatService chatService)
        {
            _notificationHubContext = notificationHubContext;
            _groupService = groupService;
            _chatService = chatService;
        }



        // POST: CreateGroup
        [HttpPost]
        public async Task<IActionResult> CreateGroup([FromForm] CreateGroup dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var group = await _groupService.CreateGroupAsync(UserId, dto);

                await _chatService.CreateChatAsync(UserId, "Group", group.Keys.First());

                foreach (var participant in group.Values.First().Participants.Keys.ToList())
                {
                    await _notificationHubContext.Clients.User(participant).SendAsync("ReceiveNewGroupProfiles", group);
                }

                return Ok(new { message = "Grup oluşturuldu." });
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



        // PUT: EditGroup
        [HttpPut("{groupId:guid}")]
        public async Task<IActionResult> EditGroup([FromRoute(Name = "groupId")] string groupId, [FromForm] CreateGroup dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var group = await _groupService.EditGroupAsync(UserId, groupId, dto);

                foreach (var participant in group.Values.First().Participants.Keys.ToList())
                {
                    await _notificationHubContext.Clients.User(participant).SendAsync("ReceiveGroupProfiles", group);
                }

                return Ok(new { message = "Grup bilgileri güncellendi." });
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



        // DELETE: LeaveGroup
        [HttpDelete("{groupId:guid}")]
        public async Task<IActionResult> LeaveGroup([FromRoute(Name = "groupId")] string groupId)
        {
            try
            {
                var group = await _groupService.LeaveGroupAsync(UserId, groupId);

                foreach (var participant in group.Values.First().Participants.Keys.ToList())
                {
                    await _notificationHubContext.Clients.User(participant).SendAsync("ReceiveGroupProfiles", group);
                }

                return Ok(new { message = "Gruptan çıkıldı." });
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
    }
}