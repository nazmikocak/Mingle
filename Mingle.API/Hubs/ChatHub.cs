using Firebase.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Mingle.Services.Abstract;
using Mingle.Services.Exceptions;
using System.Security.Claims;

namespace Mingle.API.Hubs
{
    [Authorize]
    public sealed class ChatHub : Hub
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
            var chats = await _chatService.GetChatsAsync(UserId);
            await Clients.Caller.SendAsync("ReceiveInitialChats", chats);
            await base.OnConnectedAsync();
        }


        public async Task CreateChat(string chatType, string recipientId)
        {
            try
            {
                var chatId = await _chatService.CreateChatAsync(UserId, chatType, recipientId);
                await Clients.Caller.SendAsync("ReceiveCreateChat", new { chatId = chatId });
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
                string recipientId = await _chatService.GetChatRecipientIdAsync(UserId, "Individual", chatId);
                var recipientProfile = await _userService.GetRecipientProfileAsync(recipientId);

                await Clients.Caller.SendAsync("ReceiveRecipientProfile", recipientProfile);
            }
            catch (NotFoundException ex)
            {
                await Clients.Caller.SendAsync("Error", ex.Message);
            }
            catch (BadRequestException ex)
            {
                await Clients.Caller.SendAsync("Error", ex.Message);
            }
            catch (ForbiddenException ex)
            {
                await Clients.Caller.SendAsync("Error", ex.Message);
            }
            catch (FirebaseException ex)
            {
                await Clients.Caller.SendAsync("Error", $"Firebase ile ilgili bir hata oluştu: {ex.Message}");
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Error", $"Beklenmedik bir hata oluştu: {ex.Message}");
            }
        }
    }
}