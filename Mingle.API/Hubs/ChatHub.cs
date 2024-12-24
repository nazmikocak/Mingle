using Firebase.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Mingle.DataAccess.Abstract;
using Mingle.Entities.Models;
using Mingle.Services.Abstract;
using Mingle.Services.DTOs.Response;
using Mingle.Services.Exceptions;
using System.Security.Claims;

namespace Mingle.API.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IChatService _chatService;
        private readonly IUserService _userService;



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



        public ChatHub(IChatService chatService, IUserService userService)
        {
            _chatService = chatService;
            _userService = userService;
        }



        public override async Task OnConnectedAsync()
        {
            var connectionId = Context.ConnectionId;

            var userCS = await _userService.GetConnectionSettingsAsync(UserId);

            if (!userCS.ConnectionIds.Contains(connectionId))
            {
                userCS.ConnectionIds.Add(connectionId);
                userCS.LastConnectionDate = null;

                await _userService.SaveConnectionSettingsAsync(UserId, userCS);
            }

            await base.OnConnectedAsync();
        }



        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var connectionId = Context.ConnectionId;

            var userCS = await _userService.GetConnectionSettingsAsync(UserId);

            if (!userCS.ConnectionIds.Count.Equals(0) && userCS.ConnectionIds.Contains(connectionId))
            {
                userCS.ConnectionIds.Remove(connectionId);
                userCS.LastConnectionDate = DateTime.UtcNow;

                await _userService.SaveConnectionSettingsAsync(UserId, userCS);
            }

            await base.OnDisconnectedAsync(exception);
        }



        public async Task CreateChat(string chatType, string recipientId)
        {
            try
            {
                var chatId = await _chatService.CreateChatAsync(UserId, chatType, recipientId);
                await Clients.Caller.SendAsync("ReceiveClearChat", new { chatId = chatId });
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



        public async Task ClearChat(string chatType, string chatId)
        {
            try
            {
                await _chatService.ClearChatAsync(UserId, chatType, chatId);
                await Clients.Caller.SendAsync("ReceiveClearChat", new { message = "Sohbet temizlendi." });
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



        public async Task ArchiveChat(string chatId)
        {
            try
            {
                await _chatService.ArchiveIndividualChatAsync(UserId, chatId);
                await Clients.Caller.SendAsync("ReceiveArchiveChat", new { message = "Sohbet arşivlendi." });
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



        public async Task UnarchiveChat(string chatId)
        {
            try
            {
                await _chatService.UnarchiveIndividualChatAsync(UserId, chatId);
                await Clients.Caller.SendAsync("ReceiveUnarchiveChat", new { message = "Sohbet arşivden çıkarıldı." });
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



        public async Task RecipientProfile(string chatId)
        {
            try
            {
                string recipientId = await _chatService.GetChatRecipientId(UserId, "Individual", chatId);
                var recipientProfile = await _userService.GetRecipientProfileAsync(recipientId);

                await Clients.Caller.SendAsync("ReceiveRecipientProfile", recipientProfile);
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