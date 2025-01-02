using Firebase.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Mingle.DataAccess.Abstract;
using Mingle.Entities.Models;
using Mingle.Services.Abstract;
using Mingle.Services.DTOs.Request;
using Mingle.Services.Exceptions;
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
            var chatsTask = _chatService.GetAllChatsAsync(UserId);

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
                };

            await Task.WhenAll(sendTasks);

            await base.OnConnectedAsync();
        }


        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }


        public async Task CreateChat(string chatType, string recipientId)
        {
            try
            {
                var chat = await _chatService.CreateChatAsync(UserId, chatType, recipientId);

                if (chatType.Equals("Individual"))
                {
                    var chatParticipants = chat.Values.Select(x => x.Participants).First();

                    foreach (var participant in chatParticipants)
                    {
                        await Clients.User(participant).SendAsync("ReceiveCreateChat", new Dictionary<string, Dictionary<string, Chat>> { { "Individual", chat } });
                    }

                    var recipientProfiles = await _userService.GetRecipientProfilesAsync(chatParticipants);

                    for (int i = 0; i < chatParticipants.Count; i++)
                    {
                        var profileToSend = chatParticipants[i].Equals(UserId) ? recipientProfiles[recipientId] : recipientProfiles[UserId];

                        await Clients.User(chatParticipants[i]).SendAsync("ReceiveRecipientProfiles", new Dictionary<string, object>
                            {
                                { profileToSend.Equals(recipientProfiles[chatParticipants[1]]) ? recipientId : UserId, profileToSend }
                            }
                        );
                    }
                }
                else
                {
                    var groupParticipants = await _groupService.GetGroupParticipantsAsync(UserId, chat.Values.First().Participants.First());

                    foreach (var participant in groupParticipants)
                    {
                        await Clients.User(participant).SendAsync("ReceiveCreateChat", new Dictionary<string, Dictionary<string, Chat>> { { "Group", chat } });
                    }
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
                var chat = await _chatService.ClearChatAsync(UserId, chatType, chatId);
                await Clients.User(UserId).SendAsync("ReceiveClearChat", chat);
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
                var archivedFor = await _chatService.ArchiveIndividualChatAsync(UserId, chatId);
                await Clients.User(UserId).SendAsync("ReceiveArchiveChat", archivedFor);
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
                var archivedFor = await _chatService.ArchiveIndividualChatAsync(UserId, chatId);
                await Clients.User(UserId).SendAsync("ReceiveArchiveChat", archivedFor);
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


        public async Task SendMessage(string chatType, string chatId, SendMessage dto)
        {
            try
            {
                var (message, chatParticipants) = await _messageService.SendMessageAsync(UserId, chatId, chatType, dto);

                var saveMessageTask = _messageRepository.CreateMessageAsync(UserId, chatType, chatId, message.First().Value.First().Value.First().Key, message.First().Value.First().Value.First().Value);

                foreach (var participant in chatParticipants)
                {
                    await Clients.User(participant).SendAsync("ReceiveGetMessages", message);
                }

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
                var (message, chatParticipants) = await _messageService.DeliverOrReadMessageAsync(UserId, chatType, chatId, messageId, "Deliver");

                var saveMessageTask = _messageRepository.UpdateMessageStatusAsync(chatType, chatId, message.First().Value.First().Value.First().Key, "Delivered", message.First().Value.First().Value.First().Value);

                foreach (var participant in chatParticipants)
                {
                    await Clients.User(participant).SendAsync("ReceiveGetMessages", message);
                }

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


        public async Task ReadMessage(string chatType, string chatId, string messageId)
        {
            try
            {
                var (message, chatParticipants) = await _messageService.DeliverOrReadMessageAsync(UserId, chatType, chatId, messageId, "Read");

                var saveMessageTask = _messageRepository.UpdateMessageStatusAsync(chatType, chatId, message.First().Value.First().Value.First().Key, "Read", message.First().Value.First().Value.First().Value.Status.Read);

                foreach (var participant in chatParticipants)
                {
                    await Clients.User(participant).SendAsync("ReceiveGetMessages", message);
                }

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
    }
}