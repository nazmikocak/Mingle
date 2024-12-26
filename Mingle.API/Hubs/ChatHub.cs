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
        private readonly IMessageService _messageService;


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



        public ChatHub(IChatService chatService, IUserService userService, IMessageService messageService)
        {
            _chatService = chatService;
            _userService = userService;
            _messageService = messageService;
        }


        /// <summary>
        /// Kullanıcının belirli bir sohbet türüne göre sohbetlerini alır.
        /// </summary>
        /// <param name="chatType">Alınacak sohbetlerin türü.</param>
        /// <returns>Belirtilen sohbet türüne göre kullanıcının sohbetlerini döner.</returns>
        public async Task GetChats(string chatType)
        {
            try
            {
                var chats = _chatService.GetChatsAsync(UserId, chatType);
                await Clients.Caller.SendAsync("ReceiveGetChats", chats);
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


        /*
        public async Task GetMessages(string chatType, string chatId)
        {
            try
            {
                var messages = await _messageService.GetMessagesAsync(UserId, chatId, chatType);

                await Clients.Caller.SendAsync("ReceiveGetMessages", messages);
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


        public async Task SendMessage(string chatId, SendMessage dto)
        {
            try
            {
                var message = await _messageService.SendMessageAsync(UserId, chatId, "Individual", dto);
                await Clients.Caller.SendAsync("ReceiveGetMessages", message);

                var recipientId = await _chatService.GetChatRecipientIdAsync(UserId, "Individual", chatId);
                var userCS = await _userService.GetConnectionSettingsAsync(recipientId);

                if (!userCS.ConnectionIds.Count.Equals(0))
                {
                    foreach (var connectionId in userCS.ConnectionIds)
                    {
                        await Clients.Client(connectionId).SendAsync("ReceiveGetMessages", message);
                    }
                }
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
        */
    }
}