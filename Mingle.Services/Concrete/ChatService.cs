using AutoMapper;
using Mingle.DataAccess.Abstract;
using Mingle.Entities.Models;
using Mingle.Services.Abstract;
using Mingle.Services.DTOs.Request;
using Mingle.Services.DTOs.Response;
using Mingle.Services.Exceptions;
using Mingle.Services.Utilities;
using System;
using System.Diagnostics;


namespace Mingle.Services.Concrete
{
    public sealed class ChatService : IChatService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly ICloudRepository _cloudRepository;
        private readonly IChatRepository _chatRepository;
        private readonly IUserRepository _userRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly IMapper _mapper;


        public ChatService(IMessageRepository messageRepository, ICloudRepository cloudRepository, IChatRepository chatRepository, IUserRepository userRepository, IGroupRepository groupRepository, IMapper mapper)
        {
            _messageRepository = messageRepository;
            _cloudRepository = cloudRepository;
            _chatRepository = chatRepository;
            _userRepository = userRepository;
            _groupRepository = groupRepository;
            _mapper = mapper;
        }


        public async Task<string> CreateChatAsync(string userId, string chatType, string recipientId)
        {
            FieldValidator.ValidateRequiredFields((chatType, "chatType"), (recipientId, "recipientId"));

            if (chatType.Equals("Individual"))
            {
                var user = _userRepository.GetUserByIdAsync(recipientId) ?? throw new NotFoundException("Kullanıcı bulunamadı.");

                var chatsSnapshot = await _chatRepository.GetChatsAsync(chatType);

                string? chatId = chatsSnapshot
                .Where(chat =>
                    chat.Object.Participants.Contains(userId)
                    &&
                    chat.Object.Participants.Contains(recipientId)
                )
                .Select(chat => chat.Key)
                .SingleOrDefault();

                if (String.IsNullOrEmpty(chatId))
                {
                    chatId = Guid.NewGuid().ToString();

                    var chat = new Chat
                    {
                        Participants = new List<string> { userId, recipientId },
                        CreatedDate = DateTime.UtcNow,
                    };

                    await _chatRepository.CreateChatAsync(chatType, chatId, chat);
                }

                return chatId;
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

                return chatId;
            }
            else
            {
                throw new BadRequestException("chatType geçersiz.");
            }
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
                    var deletedFor = message.Value.DeletedFor;

                    if (deletedFor != null && !deletedFor.ContainsKey(userId))
                    {
                        deletedFor.Add(userId, DateTime.UtcNow);

                        await _messageRepository.UpdateMessageDeletedForAsync(chatType, chatId, message.Key, deletedFor);
                    }
                }
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

            var chatParticipants = await _chatRepository.GetChatParticipantsAsync(chatType, chatId) ?? throw new NotFoundException("Sohbet bulunamadı.");

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


        public async Task<RecipientProfile> RecipientProfileAsync(string userId, string chatId)
        {
            FieldValidator.ValidateRequiredFields((chatId, "chatId"));

            var chatParticipants = await _chatRepository.GetChatParticipantsAsync("Individual", chatId) ?? throw new NotFoundException("Sohbet bulunamadı.");

            if (!chatParticipants.Contains(userId))
            {
                throw new ForbiddenException("Sohbet üzerinde yetkiniz yok.");
            }

            var recipientId = chatParticipants
                .FirstOrDefault(participant => !participant.Equals(userId));

            var user = _userRepository.GetUserByIdAsync(recipientId) ?? throw new NotFoundException("Kullanıcı bulunamadı.");

            var recipientProfile = _mapper.Map<RecipientProfile>(user);

            return recipientProfile;
        }


        public async Task<Dictionary<string, Dictionary<string, Chat>>> GetChatsAsync(string userId)
        {
            var individualChatsTask = _chatRepository.GetChatsAsync("Individual");
            var groupChatsTask = _chatRepository.GetChatsAsync("Group");
            var groupsTask = _groupRepository.GetAllGroupAsync();

            await Task.WhenAll(individualChatsTask, groupChatsTask, groupsTask);

            var individualChats = await individualChatsTask;
            var groupChats = await groupChatsTask;
            var groups = await groupsTask;

            var userGroupIds = new HashSet<string>(
                groups.Where(group => group.Object.Participants.ContainsKey(userId))
                      .Select(group => group.Key)
            );

            var userIndividualChats = individualChats
                .Where(chat =>
                    chat.Object.Participants.Contains(userId) &&
                    chat.Object.Messages.Values.Any(message => !message.DeletedFor!.ContainsKey(userId))
                )
                .ToDictionary(chat => chat.Key, chat => chat.Object);

            var userGroupChats = groupChats
                .Where(chat => userGroupIds.Contains(chat.Key))
                .ToDictionary(chat => chat.Key, chat => chat.Object);

            return new Dictionary<string, Dictionary<string, Chat>>
            {
                { "Individual", userIndividualChats },
                { "Group", userGroupChats }
            };
        }
    }
}