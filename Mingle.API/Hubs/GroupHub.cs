using Firebase.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Mingle.Services.Abstract;
using Mingle.Services.DTOs.Request;
using Mingle.Services.Exceptions;
using System.Security.Claims;

namespace Mingle.API.Hubs
{
    [Authorize]
    public sealed class GroupHub : Hub
    {
        private readonly IGroupService _groupService;

        private string UserId
        {
            get
            {
                var identity = Context.User!.Identity as ClaimsIdentity;
                return identity!
                    .Claims
                    .FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)!
                    .Value;
            }
        }

        public GroupHub(IGroupService groupService)
        {
            _groupService = groupService;
        }


        // TODO: Conected Methods
        // TODO: Sadece Clinents.Caller olmayacak. Aynı zamanda değişiklikler de iletilecek.

        public async Task CreateGroup(CreateGroup dto)
        {
            try
            {
                var groupId = await _groupService.CreateGroupAsync(UserId, dto);
                await Clients.Caller.SendAsync("ReceiveCreateGroup", new { groupId = groupId });
            }
            catch (NotFoundException ex)
            {
                await Clients.Caller.SendAsync("Error", new { type = "NotFound", message = ex.Message });
            }
            catch (BadRequestException ex)
            {
                await Clients.Caller.SendAsync("Error", new { type = "BadRequest", message = ex.Message });
            }
            catch (ForbiddenException ex)
            {
                await Clients.Caller.SendAsync("Error", new { type = "Forbidden", message = ex.Message });
            }
            catch (FirebaseException ex)
            {
                await Clients.Caller.SendAsync("Error", new { type = "InternalServerError", message = $"Firebase ile ilgili bir hata oluştu: {ex.Message}" });
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Error", new { type = "InternalServerError", message = $"Beklenmedik bir hata oluştu: {ex.Message}" });
            }
        }


        public async Task EditGroup(string groupId, CreateGroup dto)
        {
            try
            {
                await _groupService.EditGroupAsync(UserId, groupId, dto);
                await Clients.Caller.SendAsync("ReceiveEditGroup", new { message = "Grup bilgileri güncellendi." });
            }
            catch (NotFoundException ex)
            {
                await Clients.Caller.SendAsync("Error", new { type = "NotFound", message = ex.Message });
            }
            catch (BadRequestException ex)
            {
                await Clients.Caller.SendAsync("Error", new { type = "BadRequest", message = ex.Message });
            }
            catch (ForbiddenException ex)
            {
                await Clients.Caller.SendAsync("Error", new { type = "Forbidden", message = ex.Message });
            }
            catch (FirebaseException ex)
            {
                await Clients.Caller.SendAsync("Error", new { type = "InternalServerError", message = $"Firebase ile ilgili bir hata oluştu: {ex.Message}" });
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Error", new { type = "InternalServerError", message = $"Beklenmedik bir hata oluştu: {ex.Message}" });
            }
        }


        public async Task LeaveGroup(string groupId)
        {
            try
            {
                await _groupService.LeaveGroupAsync(UserId, groupId);
                await Clients.Caller.SendAsync("ReceiveLeaveGroup", new { message = "Gruptan çıkıldı." });
            }
            catch (NotFoundException ex)
            {
                await Clients.Caller.SendAsync("Error", new { type = "NotFound", message = ex.Message });
            }
            catch (BadRequestException ex)
            {
                await Clients.Caller.SendAsync("Error", new { type = "BadRequest", message = ex.Message });
            }
            catch (ForbiddenException ex)
            {
                await Clients.Caller.SendAsync("Error", new { type = "Forbidden", message = ex.Message });
            }
            catch (FirebaseException ex)
            {
                await Clients.Caller.SendAsync("Error", new { type = "InternalServerError", message = $"Firebase ile ilgili bir hata oluştu: {ex.Message}" });
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Error", new { type = "InternalServerError", message = $"Beklenmedik bir hata oluştu: {ex.Message}" });
            }
        }


        public async Task GetGroupProfile(string groupId)
        {
            try
            {
                var group = await _groupService.GetGroupProfileAsync(UserId, groupId);
                await Clients.Caller.SendAsync("ReceiveGetGroupProfile", group);
            }
            catch (NotFoundException ex)
            {
                await Clients.Caller.SendAsync("Error", new { type = "NotFound", message = ex.Message });
            }
            catch (BadRequestException ex)
            {
                await Clients.Caller.SendAsync("Error", new { type = "BadRequest", message = ex.Message });
            }
            catch (ForbiddenException ex)
            {
                await Clients.Caller.SendAsync("Error", new { type = "Forbidden", message = ex.Message });
            }
            catch (FirebaseException ex)
            {
                await Clients.Caller.SendAsync("Error", new { type = "InternalServerError", message = $"Firebase ile ilgili bir hata oluştu: {ex.Message}" });
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Error", new { type = "InternalServerError", message = $"Beklenmedik bir hata oluştu: {ex.Message}" });
            }
        }




    }
}
