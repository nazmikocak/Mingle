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
    public abstract class MessageHub : Hub
    {
        private readonly IMessageService _messageService;
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


        protected MessageHub(IMessageService messageService, IChatService chatService, IUserService userService)
        {
            _messageService = messageService;
            _chatService = chatService;
            _userService = userService;
        }


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


        public async Task DeliverMessage(string chatType, string chatId, string messageId)
        {
            try
            {
                var message = await _messageService.DeliverOrReadMessageAsync(UserId, chatType, chatId, messageId, "Delivered");
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


        public async Task ReadMessage(string chatType, string chatId, string messageId)
        {
            try
            {
                var message = await _messageService.DeliverOrReadMessageAsync(UserId, chatType, chatId, messageId, "Read");
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
    }
}