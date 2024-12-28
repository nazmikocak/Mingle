﻿using Mingle.DataAccess.Abstract;
using Mingle.Entities.Models;
using Mingle.Services.Abstract;
using Mingle.Services.Exceptions;
using Mingle.Services.Utilities;


namespace Mingle.Services.Concrete
{
    public sealed class ChatService : IChatService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly IChatRepository _chatRepository;
        private readonly IUserRepository _userRepository;


        public ChatService(IMessageRepository messageRepository, IGroupRepository groupRepository, IChatRepository chatRepository, IUserRepository userRepository)
        {
            _messageRepository = messageRepository;
            _groupRepository = groupRepository;
            _chatRepository = chatRepository;
            _userRepository = userRepository;
        }


        public async Task<Dictionary<string, Chat>> CreateChatAsync(string userId, string chatType, string recipientId)
        {
            FieldValidator.ValidateRequiredFields((chatType, "chatType"), (recipientId, "recipientId"));

            if (chatType.Equals("Individual"))
            {
                var user = _userRepository.GetUserByIdAsync(recipientId) ?? throw new NotFoundException("Kullanıcı bulunamadı.");

                var chatsSnapshot = await _chatRepository.GetChatsAsync(chatType);

                var oldChat = chatsSnapshot
                    .Where(chat =>
                        chat.Object.Participants.Contains(userId)
                        &&
                        chat.Object.Participants.Contains(recipientId)
                    )
                    .ToDictionary(
                        chat => chat.Key,
                        chat => chat.Object
                    );

                if (oldChat.Count.Equals(0))
                {
                    string chatId = Guid.NewGuid().ToString();

                    var newchat = new Chat
                    {
                        Participants = [userId, recipientId],
                        CreatedDate = DateTime.UtcNow,
                    };

                    await _chatRepository.CreateChatAsync(chatType, chatId, newchat);

                    return new Dictionary<string, Chat> { { chatId, newchat } };
                }

                return oldChat;
            }
            else if (chatType.Equals("Group"))
            {
                string? chatId = Guid.NewGuid().ToString();

                var chat = new Chat
                {
                    Participants = [recipientId],
                    CreatedDate = DateTime.UtcNow,
                };

                await _chatRepository.CreateChatAsync(chatType, chatId, chat);

                return new Dictionary<string, Chat> { { chatId, chat } };
            }
            else
            {
                throw new BadRequestException("chatType geçersiz.");
            }
        }


        public async Task<(Dictionary<string, Dictionary<string, Chat>>, List<string>, List<string>)> GetAllChatsAsync(string userId)
        {
            var individualChatsTask = _chatRepository.GetChatsAsync("Individual");
            var groupChatsTask = _chatRepository.GetChatsAsync("Group");
            var groupsTask = _groupRepository.GetAllGroupAsync();

            await Task.WhenAll(individualChatsTask, groupChatsTask, groupsTask);

            var individualChats = await individualChatsTask;
            var groupChats = await groupChatsTask;
            var groups = await groupsTask;

            var userGroupIds = new List<string>(
                groups.Where(group => group.Object.Participants.ContainsKey(userId))
                      .Select(group => group.Key)
            );

            var userIndividualChats = individualChats
                .Where(chat =>
                    chat.Object.Participants.Contains(userId) &&
                    chat.Object.Messages.Values.Any(message => !message.DeletedFor!.ContainsKey(userId))
                )
                .ToDictionary(
                    chat => chat.Key,
                    chat => new Chat
                    {
                        Participants = chat.Object.Participants,
                        ArchivedFor = chat.Object.ArchivedFor,
                        CreatedDate = chat.Object.CreatedDate,
                        Messages = chat.Object.Messages
                            .OrderBy(x => x.Value.Status.Sent.Values.First())
                            .ToDictionary(x => x.Key, x => x.Value)
                    }
                );

            var userGroupChats = groupChats
                .Where(chat => userGroupIds.Contains(chat.Key))
                .ToDictionary(chat => chat.Key, chat => chat.Object);

            var chatsRecipientIds = userIndividualChats
                .Select(chat => chat.Value.Participants.FirstOrDefault(participant => !participant.Equals(userId))!)
                .ToList();

            return (new Dictionary<string, Dictionary<string, Chat>>
            {
                { "Individual", userIndividualChats },
                { "Group", userGroupChats }
            },
            chatsRecipientIds,
            userGroupIds
            );
        }


        public async Task ClearChatAsync(string userId, string chatType, string chatId)
        {
            FieldValidator.ValidateRequiredFields((chatType, "chatType"), (chatId, "chatId"));

            var chat = await _chatRepository.GetChatByIdAsync(chatType, chatId) ?? throw new NotFoundException("Sohbet bulunamadı.");

            if (!chat.Participants.Contains(userId))
            {
                throw new ForbiddenException("Sohbet üzerinde yetkiniz yok.");
            }

            if (chat.Messages != null)
            {
                foreach (var message in chat.Messages)
                {
                    if (!message.Value.DeletedFor!.ContainsKey(userId))
                    {
                        message.Value.DeletedFor.Add(userId, DateTime.UtcNow);
                    }
                }

                await _chatRepository.UpdateChatMessageAsync(chatType, chatId, chat.Messages);
            }
            else
            {
                throw new BadRequestException("Henüz silebileceğiniz bir mesaj yok.");
            }
        }


        public async Task ArchiveIndividualChatAsync(string userId, string chatId)
        {
            FieldValidator.ValidateRequiredFields((chatId, chatId));

            var chat = await _chatRepository.GetChatByIdAsync("Individual", chatId) ?? throw new NotFoundException("Sohbet bulunamadı.");

            if (!chat.Participants.Contains(userId))
            {
                throw new ForbiddenException("Sohbet üzerinde yetkiniz yok.");
            }

            if (!chat.ArchivedFor.ContainsKey(userId))
            {
                var archivedFor = chat.ArchivedFor;
                archivedFor.Add(userId, DateTime.UtcNow);

                await _chatRepository.UpdateChatArchivedForAsync("Individual", chatId, archivedFor);
            }
            else
            {
                throw new BadRequestException("Sohbet zaten arşivlenmiş.");
            }
        }


        public async Task UnarchiveIndividualChatAsync(string userId, string chatId)
        {
            FieldValidator.ValidateRequiredFields((chatId, "chatId"));

            var chat = await _chatRepository.GetChatByIdAsync("Individual", chatId) ?? throw new NotFoundException("Sohbet bulunamadı.");

            if (!chat.Participants.Contains(userId))
            {
                throw new ForbiddenException("Sohbet üzerinde yetkiniz yok.");
            }

            if (chat.ArchivedFor.ContainsKey(userId))
            {
                var archivedFor = chat.ArchivedFor;
                archivedFor.Remove(userId);

                await _chatRepository.UpdateChatArchivedForAsync("Individual", chatId, archivedFor);
            }
            else
            {
                throw new BadRequestException("Sohbet zaten arşivde değil.");
            }
        }


        public async Task<string> GetChatRecipientIdAsync(string userId, string chatType, string chatId)
        {
            FieldValidator.ValidateRequiredFields((chatType, "chatType"), (chatId, "chatId"));

            var chatParticipants = await _chatRepository.GetChatParticipantsByIdAsync(chatType, chatId) ?? throw new NotFoundException("Sohbet bulunamadı.");

            string recipientId;

            if (chatType.Equals("Individual"))
            {
                if (!chatParticipants.Contains(userId))
                {
                    throw new ForbiddenException("Sohbet üzerinde yetkiniz yok.");
                }

                recipientId = chatParticipants.FirstOrDefault(participant => !participant.Equals(userId))!;
            }
            else if (chatType.Equals("Group"))
            {
                recipientId = chatParticipants[0];
            }
            else
            {
                throw new BadRequestException("chatType geçersiz.");
            }

            return recipientId;
        }
    }
}