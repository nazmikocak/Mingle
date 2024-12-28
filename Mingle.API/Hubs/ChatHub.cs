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
    public sealed class ChatHub : Hub
    {
        private readonly IChatService _chatService;
        private readonly IUserService _userService;
        private readonly IGroupService _groupService;
        private readonly IMessageService _messageService;
        private readonly IMessageRepository _messageRepository;


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


        public ChatHub(IChatService chatService, IUserService userService, IGroupService groupService, IMessageService messageService, IMessageRepository messageRepository)
        {
            _chatService = chatService;
            _userService = userService;
            _groupService = groupService;
            _messageService = messageService;
            _messageRepository = messageRepository;
        }


        public override async Task OnConnectedAsync()
        {
            var connectionId = Context.ConnectionId;

            var userCSTask = _userService.GetConnectionSettingsAsync(UserId);
            var chatsTask = _chatService.GetAllChatsAsync(UserId);

            var userCS = await userCSTask;

            if (!userCS.ConnectionIds.Contains(connectionId))
            {
                userCS.ConnectionIds.Add(connectionId);
                userCS.LastConnectionDate = null;

                var saveSettingsTask = _userService.SaveConnectionSettingsAsync(UserId, userCS);

                var (chats, chatsRecipientIds, userGroupIds) = await chatsTask;

                var recipientProfilesTask = _userService.GetRecipientProfilesAsync(chatsRecipientIds);
                var groupProfilesTask = _groupService.GetGroupProfilesAsync(userGroupIds);

                var recipientProfiles = await recipientProfilesTask;
                var groupProfiles = await groupProfilesTask;

                var sendTasks = new[]
                {
                Clients.Caller.SendAsync("ReceiveInitialChats", chats),
                Clients.Caller.SendAsync("ReceiveInitialGroupProfiles", groupProfiles),
                Clients.Caller.SendAsync("ReceiveInitialRecipientProfiles", recipientProfiles),
                Clients.All.SendAsync("ReceiveRecipientProfiles", userCS)
                };

                await Task.WhenAll(sendTasks);
                await saveSettingsTask;
            }

            await base.OnConnectedAsync();
        }



        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var connectionId = Context.ConnectionId;

            var userCS = await _userService.GetConnectionSettingsAsync(UserId);

            if (userCS.ConnectionIds.Remove(connectionId))
            {
                userCS.LastConnectionDate = DateTime.UtcNow;

                var saveSettingsTask = _userService.SaveConnectionSettingsAsync(UserId, userCS);
                var notifyClientsTask = Clients.All.SendAsync("ReceiveRecipientProfiles", userCS);

                await Task.WhenAll(saveSettingsTask, notifyClientsTask);
            }

            await base.OnDisconnectedAsync(exception);
        }


        public async Task CreateChat(string chatType, string recipientId)
        {
            try
            {
                var chat = await _chatService.CreateChatAsync(UserId, chatType, recipientId);

                if (chatType.Equals("Individual"))
                {
                    List<string> chatParticipants = new() { UserId, recipientId };
                    var userConnectionIds = await _userService.GetUserConnectionIdsAsync(chatParticipants);


                    var groupTasks = userConnectionIds.SelectMany(user =>
                        user.Select(connectionId => Groups.AddToGroupAsync(connectionId, chat.Keys.First()))
                    );

                    await Task.WhenAll(groupTasks);

                    await Clients.Group(chat.Keys.First()).SendAsync("ReceiveCreateChat", new Dictionary<string, Dictionary<string, Chat>> { { "Individual", chat } });

                    var recipientProfile = await _userService.GetRecipientProfileByIdAsync(recipientId);
                    await Clients.Group(chat.Keys.First()).SendAsync("ReceiveRecipientProfile", recipientProfile);
                }

                else
                {
                    var groupParticipants = await _groupService.GetGroupParticipantsAsync(UserId, chat.Values.First().Participants.First());
                    var userConnectionIds = await _userService.GetUserConnectionIdsAsync(groupParticipants);

                    foreach (var user in userConnectionIds)
                    {
                        foreach (var connectionId in user)
                        {
                            await Groups.AddToGroupAsync(connectionId, chat.Keys.First());
                        }
                    }

                    await Clients.Group(chat.Keys.First()).SendAsync("ReceiveCreateChat", chat);

                    var groupProfile = await _groupService.GetGroupProfileByIdAsync(UserId, chat.Values.First().Participants.First());

                    await Clients.Group(chat.Keys.First()).SendAsync("ReceiveGroupProfiles", groupProfile);
                }
            }
            catch (Exception ex) when (
                ex is NotFoundException ||
                ex is BadRequestException ||
                ex is ForbiddenException ||
                ex is FirebaseException)
            {
                await Clients.Caller.SendAsync("Error", new { message = ex.Message });
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Error", new { message = $"Beklenmedik bir hata oluştu: {ex.Message}" });
            }
        }


        public async Task ClearChat(string chatType, string chatId)
        {
            try
            {
                await _chatService.ClearChatAsync(UserId, chatType, chatId);
                await Clients.Caller.SendAsync("ReceiveClearChat", new { message = "Sohbet temizlendi." });
            }
            catch (Exception ex) when (
                ex is NotFoundException ||
                ex is BadRequestException ||
                ex is ForbiddenException ||
                ex is FirebaseException)
            {
                await Clients.Caller.SendAsync("Error", new { message = ex.Message });
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Error", new { message = $"Beklenmedik bir hata oluştu: {ex.Message}" });
            }
        }


        public async Task ArchiveChat(string chatId)
        {
            try
            {
                await _chatService.ArchiveIndividualChatAsync(UserId, chatId);
                await Clients.Caller.SendAsync("ReceiveArchiveChat", new { message = "Sohbet arşivlendi." });
            }
            catch (Exception ex) when (
                ex is NotFoundException ||
                ex is BadRequestException ||
                ex is ForbiddenException ||
                ex is FirebaseException)
            {
                await Clients.Caller.SendAsync("Error", new { message = ex.Message });
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Error", new { message = $"Beklenmedik bir hata oluştu: {ex.Message}" });
            }
        }



        public async Task UnarchiveChat(string chatId)
        {
            try
            {
                await _chatService.UnarchiveIndividualChatAsync(UserId, chatId);
                await Clients.Caller.SendAsync("ReceiveUnarchiveChat", new { message = "Sohbet arşivden çıkarıldı." });
            }
            catch (Exception ex) when (
                ex is NotFoundException ||
                ex is BadRequestException ||
                ex is ForbiddenException ||
                ex is FirebaseException)
            {
                await Clients.Caller.SendAsync("Error", new { message = ex.Message });
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Error", new { message = $"Beklenmedik bir hata oluştu: {ex.Message}" });
            }
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

                List<string> chatParticipants = [UserId, recipientId];

                var userConnectionIdsTask = _userService.GetUserConnectionIdsAsync(chatParticipants);

                var saveMessageTask = _messageRepository.CreateMessageAsync(
                    UserId,
                    "Individual",
                    chatId,
                    message.Keys.First(),
                    message.Values.First()
                );

                var userConnectionIds = await userConnectionIdsTask;

                var addToGroupTasks = userConnectionIds
                    .SelectMany(user => user)
                    .Select(connectionId => Groups.AddToGroupAsync(connectionId, chatId));

                await Task.WhenAll(addToGroupTasks);

                await Clients.Group(chatId).SendAsync("ReceiveGetMessages", new Dictionary<string, Dictionary<string, Dictionary<string, Message>>> { { "Individual", messageVM } });

                await saveMessageTask;
            }
            catch (Exception ex) when (
                ex is NotFoundException ||
                ex is BadRequestException ||
                ex is ForbiddenException ||
                ex is FirebaseException)
            {
                await Clients.Caller.SendAsync("Error", new { message = ex.Message });
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

                List<string> chatParticipants = [UserId, recipientId];
                var userConnectionIds = await _userService.GetUserConnectionIdsAsync(chatParticipants);

                foreach (var user in userConnectionIds)
                {
                    foreach (var connectionId in user)
                    {
                        await Groups.AddToGroupAsync(connectionId, chatId);
                    }
                }

                await Clients.Group(chatId).SendAsync("ReceiveGetMessages", new Dictionary<string, Dictionary<string, Dictionary<string, Message>>> { { "Individual", messageVM } });

                await _messageRepository.CreateMessageAsync(UserId, chatType, chatId, message.Keys.First(), message.Values.First());
            }
            catch (Exception ex) when (
                ex is NotFoundException ||
                ex is BadRequestException ||
                ex is ForbiddenException ||
                ex is FirebaseException)
            {
                await Clients.Caller.SendAsync("Error", new { message = ex.Message });
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

                List<string> chatParticipants = [UserId, recipientId];
                var userConnectionIds = await _userService.GetUserConnectionIdsAsync(chatParticipants);

                foreach (var user in userConnectionIds)
                {
                    foreach (var connectionId in user)
                    {
                        await Groups.AddToGroupAsync(connectionId, chatId);
                    }
                }

                await Clients.Group(chatId).SendAsync("ReceiveGetMessages", new Dictionary<string, Dictionary<string, Dictionary<string, Message>>> { { "Individual", messageVM } });

                await _messageRepository.CreateMessageAsync(UserId, chatType, chatId, message.Keys.First(), message.Values.First());
            }
            catch (Exception ex) when (
                ex is NotFoundException ||
                ex is BadRequestException ||
                ex is ForbiddenException ||
                ex is FirebaseException)
            {
                await Clients.Caller.SendAsync("Error", new { message = ex.Message });
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Error", new { message = $"Beklenmedik bir hata oluştu: {ex.Message}" });
            }
        }

    }
}