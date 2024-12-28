using Firebase.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Mingle.DataAccess.Abstract;
using Mingle.Entities.Enums;
using Mingle.Entities.Models;
using Mingle.Services.Abstract;
using Mingle.Services.Concrete;
using Mingle.Services.DTOs.Request;
using Mingle.Services.DTOs.Response;
using Mingle.Services.Exceptions;
using System;
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
            var (chats, userChatIds, chatsRecipientIds, userGroupIds) = await _chatService.GetChatsAsync(UserId);

            foreach (var chatId in userChatIds)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, chatId);
            }

            var recipientProfiles = await _userService.GetRecipientProfilesAsync(chatsRecipientIds);
            var groupProfiles = await _groupService.GetGroupProfilesAsync(userGroupIds);

            await Clients.Caller.SendAsync("ReceiveInitialChats", chats);
            await Clients.Caller.SendAsync("ReceiveGroupProfiles", groupProfiles);
            await Clients.Caller.SendAsync("ReceiveRecipientProfiles", recipientProfiles);

            await base.OnConnectedAsync();
        }


        public async override Task OnDisconnectedAsync(Exception? exception)
        {
            var (_, userChatIds, _, _) = await _chatService.GetChatsAsync(UserId);

            foreach (var chatId in userChatIds)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId);
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
                    List<string> chatParticipants = [UserId, recipientId];
                    var userConnectionIds = await _userService.GetUserConnectionIdsAsync(chatParticipants);

                    foreach (var user in userConnectionIds)
                    {
                        foreach (var connectionId in user)
                        {
                            await Groups.AddToGroupAsync(connectionId, chat.Keys.First());
                        }
                    }

                    var recipientProfile = await _userService.GetRecipientProfileAsync(recipientId);

                    await Clients.Group(chat.Keys.First()).SendAsync("ReceiveCreateChat", chat);
                    await Clients.Group(chat.Keys.First()).SendAsync("ReceiveRecipientProfiles", recipientProfile);

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

                    var groupProfile = await _groupService.GetGroupProfileAsync(UserId, chat.Values.First().Participants.First());

                    await Clients.Group(chat.Keys.First()).SendAsync("ReceiveCreateChat", chat);
                    await Clients.Group(chat.Keys.First()).SendAsync("ReceiveGroupProfiles", groupProfile);
                }
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


        public async Task ClearChat(string chatType, string chatId)
        {
            try
            {
                await _chatService.ClearChatAsync(UserId, chatType, chatId);
                await Clients.Caller.SendAsync("ReceiveClearChat", new { message = "Sohbet temizlendi." });
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


        public async Task ArchiveChat(string chatId)
        {
            try
            {
                await _chatService.ArchiveIndividualChatAsync(UserId, chatId);
                await Clients.Caller.SendAsync("ReceiveArchiveChat", new { message = "Sohbet arşivlendi." });
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


        public async Task UnarchiveChat(string chatId)
        {
            try
            {
                await _chatService.UnarchiveIndividualChatAsync(UserId, chatId);
                await Clients.Caller.SendAsync("ReceiveUnarchiveChat", new { message = "Sohbet arşivden çıkarıldı." });
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
                var userConnectionIds = await _userService.GetUserConnectionIdsAsync(chatParticipants);

                foreach (var user in userConnectionIds)
                {
                    foreach (var connectionId in user)
                    {
                        await Groups.AddToGroupAsync(connectionId, chatId);
                    }
                }

                await Clients.Group(chatId).SendAsync("ReceiveGetMessages", new { Individual = messageVM });

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
    }
}