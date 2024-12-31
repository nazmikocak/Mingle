using Firebase.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Mingle.API.Hubs;
using Mingle.Services.Abstract;
using Mingle.Services.DTOs.Request;
using Mingle.Services.Exceptions;

namespace Mingle.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class GroupController : BaseController
    {
        private readonly IHubContext<NotificationHub> _notificationHubContext;
        private readonly IGroupService _groupService;



        public GroupController(IHubContext<NotificationHub> notificationHubContext, IGroupService groupService)
        {
            _notificationHubContext = notificationHubContext;
            _groupService = groupService;
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
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Firebase ile ilgili bir hata oluştu: {ex.Message}" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Beklenmedik bir hata oluştu: {ex.Message}" });
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
                var group = await _groupService.CreateGroupAsync(UserId, dto);

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
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Firebase ile ilgili bir hata oluştu: {ex.Message}" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Beklenmedik bir hata oluştu: {ex.Message}" });
            }
        }



        // TODO: Yanıt şekli. Hub üzerinden iletilecek olan?
        // DELETE: LeaveGroup
        [HttpDelete("{groupId:guid}")]
        public async Task<IActionResult> LeaveGroup([FromRoute(Name = "groupId")] string groupId)
        {
            try
            {
                var group = await _groupService.LeaveGroupAsync(UserId, groupId);

                // Kullanıcı ChatHUb daki gruplardan da çıkarılmalı.

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
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Firebase ile ilgili bir hata oluştu: {ex.Message}" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Beklenmedik bir hata oluştu: {ex.Message}" });
            }
        }
    }
}