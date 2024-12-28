using Firebase.Auth;
using Firebase.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Mingle.DataAccess.Abstract;
using Mingle.Entities.Models;
using Mingle.Services.Abstract;
using Mingle.Services.DTOs.Request;
using Mingle.Services.Exceptions;
using System.Diagnostics;
using System.Security.Claims;

namespace Mingle.API.Hubs
{
    [Authorize]
    public sealed class MessageHub : Hub
    {
        private readonly IMessageService _messageService;
        private readonly IMessageRepository _messageRepository;
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


        public MessageHub(IMessageService messageService, IMessageRepository messageRepository, IChatService chatService, IUserService userService)
        {
            _messageService = messageService;
            _messageRepository = messageRepository;
            _chatService = chatService;
            _userService = userService;
        }


        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }


        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }


        public async Task SendDemoMessage(string chatId, SendMessage dto)
        {
            var (message, recipientId) = await _messageService.SendMessageAsync(UserId, chatId, "Individual", dto);

            var createMessageTask = _messageRepository.CreateMessageAsync(UserId, "Individual", chatId, message.Keys.First(), message.Values.First());

            await Clients.Caller.SendAsync("ReceiveGetMessages", new Dictionary<string, Dictionary<string, Message>>
                {
                    { chatId, message }
                }
            );

            var userCS = await _userService.GetConnectionSettingsAsync(recipientId);

            if (!userCS.ConnectionIds.Count.Equals(0))
            {
                var sendTasks = userCS.ConnectionIds
                    .Select(connectionId => Clients.Client(connectionId).SendAsync("ReceiveGetMessages", new Dictionary<string, Dictionary<string, Message>>
                    {
                        { chatId, message }
                    }));
                await Task.WhenAll(sendTasks);
            }

            await createMessageTask;
        }


        public async Task SendMessage(string chatId, SendMessage dto)
        {
            try
            {
                var (message, recipientId) = await _messageService.SendMessageAsync(UserId, chatId, "Individual", dto);

                var messageVM = new Dictionary<string, Dictionary<string, Message>>
                {
                    { chatId, message }
                };

                await Clients.Caller.SendAsync("ReceiveGetMessages", messageVM);


                var userCS = await _userService.GetConnectionSettingsAsync(recipientId);

                if (!userCS.ConnectionIds.Count.Equals(0))
                {
                    foreach (var connectionId in userCS.ConnectionIds)
                    {
                        await Clients.Client(connectionId).SendAsync("ReceiveGetMessages", messageVM);
                    }
                }

                await _messageRepository.CreateMessageAsync(UserId, "Individual", chatId, message.Keys.First(), message.Values.First());
            }
            catch (NotFoundException ex)
            {
                await Clients.Caller.SendAsync("Error", new { message = ex.Message });
            }
            catch (BadRequestException ex)
            {
                await Clients.Caller.SendAsync("Error", new { message = ex.Message });
            }
            catch (ForbiddenException ex)
            {
                await Clients.Caller.SendAsync("Error", new { message = ex.Message });
            }
            catch (FirebaseException ex)
            {
                await Clients.Caller.SendAsync("Error", new { message = $"Firebase ile ilgili bir hata oluştu: {ex.Message}" });
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Error", new { message = $"Beklenmedik bir hata oluştu: {ex.Message}" });
            }
        }


        public async Task DeliverMessage(string chatType, string chatId, string messageId)
        {
            try
            {
                var (message, recipientId) = await _messageService.DeliverOrReadMessageAsync(UserId, chatType, chatId, messageId, "Delivered");

                var messageVM = new Dictionary<string, Dictionary<string, Message>>
                {
                    { chatId, message }
                };

                await Clients.Caller.SendAsync("ReceiveGetMessages", messageVM);

                var userCS = await _userService.GetConnectionSettingsAsync(recipientId);

                if (!userCS.ConnectionIds.Count.Equals(0))
                {
                    foreach (var connectionId in userCS.ConnectionIds)
                    {
                        await Clients.Client(connectionId).SendAsync("ReceiveGetMessages", messageVM);
                    }
                }

                await _messageRepository.UpdateMessageStatusAsync(chatType, chatId, messageId, "Delivered", message.Values.Select(x => x.Status.Delivered.Keys.SingleOrDefault(x => x.Equals(UserId))));
            }
            catch (NotFoundException ex)
            {
                await Clients.Caller.SendAsync("Error", new { message = ex.Message });
            }
            catch (BadRequestException ex)
            {
                await Clients.Caller.SendAsync("Error", new { message = ex.Message });
            }
            catch (ForbiddenException ex)
            {
                await Clients.Caller.SendAsync("Error", new { message = ex.Message });
            }
            catch (FirebaseException ex)
            {
                await Clients.Caller.SendAsync("Error", new { message = $"Firebase ile ilgili bir hata oluştu: {ex.Message}" });
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Error", new { message = $"Beklenmedik bir hata oluştu: {ex.Message}" });
            }
        }


        public async Task ReadMessage(string chatType, string chatId, string messageId)
        {
            try
            {
                var (message, recipientId) = await _messageService.DeliverOrReadMessageAsync(UserId, chatType, chatId, messageId, "Read");

                var messageVM = new Dictionary<string, Dictionary<string, Message>>
                {
                    { chatId, message }
                };

                await Clients.Caller.SendAsync("ReceiveGetMessages", messageVM);

                var userCS = await _userService.GetConnectionSettingsAsync(recipientId);

                if (!userCS.ConnectionIds.Count.Equals(0))
                {
                    foreach (var connectionId in userCS.ConnectionIds)
                    {
                        await Clients.Client(connectionId).SendAsync("ReceiveGetMessages", messageVM);
                    }
                }

                await _messageRepository.UpdateMessageStatusAsync(chatType, chatId, messageId, "Read", message.Values.Select(x => x.Status.Read.Keys.SingleOrDefault(x => x.Equals(UserId))));
            }
            catch (NotFoundException ex)
            {
                await Clients.Caller.SendAsync("Error", new { message = ex.Message });
            }
            catch (BadRequestException ex)
            {
                await Clients.Caller.SendAsync("Error", new { message = ex.Message });
            }
            catch (ForbiddenException ex)
            {
                await Clients.Caller.SendAsync("Error", new { message = ex.Message });
            }
            catch (FirebaseException ex)
            {
                await Clients.Caller.SendAsync("Error", new { message = $"Firebase ile ilgili bir hata oluştu: {ex.Message}" });
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Error", new { message = $"Beklenmedik bir hata oluştu: {ex.Message}" });
            }
        }


        public async Task DeleteMessage(string chatType, string chatId, string messageId, byte deletionType) 
        {
            try
            {
                await _messageService.DeleteMessageAsync(UserId, chatType, chatId, messageId, deletionType);
            }
            catch (NotFoundException ex)
            {
                await Clients.Caller.SendAsync("Error", new { message = ex.Message });
            }
            catch (BadRequestException ex)
            {
                await Clients.Caller.SendAsync("Error", new { message = ex.Message });
            }
            catch (ForbiddenException ex)
            {
                await Clients.Caller.SendAsync("Error", new { message = ex.Message });
            }
            catch (FirebaseException ex)
            {
                await Clients.Caller.SendAsync("Error", new { message = $"Firebase ile ilgili bir hata oluştu: {ex.Message}" });
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Error", new { message = $"Beklenmedik bir hata oluştu: {ex.Message}" });
            }
        }
    }
}