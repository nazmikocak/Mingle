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
    /// Grup ile ilgili işlemleri gerçekleştiren API denetleyicisidir.
    /// Kullanıcıların gruplar oluşturması, gruptan çıkması ve grup bilgilerini güncellemesi gibi işlemler yapılabilir.
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public sealed class GroupController : BaseController
    {
        private readonly IHubContext<NotificationHub> _notificationHubContext;
        private readonly IGroupService _groupService;
        private readonly IChatService _chatService;



        /// <summary>
        /// GroupController sınıfının yapıcı metodudur.
        /// Gerekli servisleri alarak controller'ı başlatır.
        /// </summary>
        /// <param name="notificationHubContext">Bildirim hub'ı için <see cref="IHubContext{NotificationHub}"/> nesnesi.</param>
        /// <param name="groupService">Grup işlemleri için <see cref="IGroupService"/> nesnesi.</param>
        /// <param name="chatService">Sohbet işlemleri için <see cref="IChatService"/> nesnesi.</param>
        public GroupController(IHubContext<NotificationHub> notificationHubContext, IGroupService groupService, IChatService chatService)
        {
            _notificationHubContext = notificationHubContext;
            _groupService = groupService;
            _chatService = chatService;
        }



        /// <summary>
        /// Yeni bir grup oluşturur.
        /// </summary>
        /// <param name="dto">Grup oluşturma işlemi için gerekli verileri içeren <see cref="Create"/> veri transfer nesnesi.</param>
        /// <returns>Yeni oluşturulan grubun bilgileri ile birlikte başarı durumu döner.</returns>
        /// <exception cref="BadRequestException">Model geçerli değilse hata mesajı döner.</exception>
        /// <exception cref="FirebaseException">Firebase ile ilgili bir hata oluştuğunda hata mesajı döner.</exception>
        /// <exception cref="Exception">Beklenmedik bir hata oluştuğunda hata mesajı döner.</exception>
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateGroup dto)
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



        /// <summary>
        /// Var olan bir grubun bilgilerini günceller.
        /// </summary>
        /// <param name="groupId">Düzenlenecek grubun kimliği.</param>
        /// <param name="dto">Grup düzenleme işlemi için gerekli verileri içeren <see cref="Create"/> veri transfer nesnesi.</param>
        /// <returns>Grup bilgileri başarıyla güncellendi mesajı döner.</returns>
        /// <exception cref="BadRequestException">Model geçerli değilse hata mesajı döner.</exception>
        /// <exception cref="FirebaseException">Firebase ile ilgili bir hata oluştuğunda hata mesajı döner.</exception>
        /// <exception cref="Exception">Beklenmedik bir hata oluştuğunda hata mesajı döner.</exception>
        [HttpPut("{groupId:guid}")]
        public async Task<IActionResult> Edit([FromRoute(Name = "groupId")] string groupId, [FromForm] CreateGroup dto)
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



        /// <summary>
        /// Var olan bir grubun bilgilerini günceller.
        /// </summary>
        /// <param name="groupId">Düzenlenecek grubun kimliği.</param>
        /// <param name="dto">Grup düzenleme işlemi için gerekli verileri içeren <see cref="Create"/> veri transfer nesnesi.</param>
        /// <returns>Grup bilgileri başarıyla güncellendi mesajı döner.</returns>
        /// <exception cref="BadRequestException">Model geçerli değilse hata mesajı döner.</exception>
        /// <exception cref="FirebaseException">Firebase ile ilgili bir hata oluştuğunda hata mesajı döner.</exception>
        /// <exception cref="Exception">Beklenmedik bir hata oluştuğunda hata mesajı döner.</exception>
        [HttpDelete("{groupId:guid}")]
        public async Task<IActionResult> Leave([FromRoute(Name = "groupId")] string groupId)
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