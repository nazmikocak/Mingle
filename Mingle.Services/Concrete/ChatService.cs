using AutoMapper;
using Mingle.DataAccess.Abstract;
using Mingle.Entities.Models;
using Mingle.Services.Abstract;
using Mingle.Services.DTOs.Request;
using Mingle.Services.DTOs.Response;
using Mingle.Services.Exceptions;

namespace Mingle.Services.Concrete
{
    public sealed class ChatService : IChatService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly ICloudRepository _cloudRepository;
        private readonly IChatRepository _chatRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;


        public ChatService(IMessageRepository messageRepository, ICloudRepository cloudRepository, IChatRepository chatRepository, IUserRepository userRepository, IMapper mapper)
        {
            _messageRepository = messageRepository;
            _cloudRepository = cloudRepository;
            _chatRepository = chatRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }


        public async Task<string> CreateChatAsync(string userId, string chatType, string recipientId)
        {
            if (String.IsNullOrEmpty(recipientId))
            {
                throw new BadRequestException("recipientId gereklidir.");
            }
            if (String.IsNullOrEmpty(chatType))
            {
                throw new BadRequestException("chatType gereklidir.");
            }

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

                    return chatId;
                }
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

            throw new BadRequestException("chatType geçersiz.");
        }


        public async Task<Chat> GetChatAsync(string userId, string chatType, string chatId)
        {
            if (String.IsNullOrEmpty(chatId))
            {
                throw new BadRequestException("chatId gereklidir.");
            }
            if (String.IsNullOrEmpty(chatType))
            {
                throw new BadRequestException("chatType gereklidir.");
            }
            if (!(chatType.Equals("Individual") || chatType.Equals("Group")))
            {
                throw new BadRequestException("chatType geçersiz.");
            }

            var chat = await _chatRepository.GetChatByIdAsync(chatType, chatId) ?? throw new NotFoundException("Sohbet bulunamadı.");

            return chat;
        }


        public async Task ClearChatAsync(string userId, string chatType, string chatId)
        {
            if (String.IsNullOrEmpty(chatId))
            {
                throw new BadRequestException("chatId gereklidir.");
            }

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
            if (String.IsNullOrEmpty(chatId))
            {
                throw new BadRequestException("chatId gereklidir.");
            }

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
            if (String.IsNullOrEmpty(chatId))
            {
                throw new BadRequestException("chatId gereklidir.");
            }

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


        public async Task SendMessageAsync(string userId, SendMessage dto)
        {
            if (!(String.IsNullOrEmpty(dto.TextContent) ^ dto.FileContent == null))
            {
                throw new BadRequestException("TextContent veya FileContent gereklidir.");
            }

            var chat = await _chatRepository.GetChatByIdAsync("Individual", dto.ChatId) ?? throw new NotFoundException("Sohbet bulunamadı.");

            if (!chat.Participants.Contains(userId))
            {
                throw new ForbiddenException("Sohbet üzerinde yetkiniz yok.");
            }

            string messageId = Guid.NewGuid().ToString();
            string messageContent;

            if (dto.FileContent != null)
            {
                var fileUrl = await _cloudRepository.UploadPhotoAsync(userId, $"Chats/{dto.ChatId}", "image_message", dto.FileContent);
                messageContent = fileUrl.ToString();
            }
            else
            {
                messageContent = dto.TextContent;
            }

            var message = new Message
            {
                Content = messageContent,
                Type = dto.ContentType,
                Status = new MessageStatus
                {
                    Sent = new Dictionary<string, DateTime>
                    {
                        { userId, DateTime.UtcNow }
                    }
                }
            };

            await _messageRepository.CreateMessageAsync("Individual", dto.ChatId, messageId, message);
        }


        public async Task<Dictionary<string, Message>> GetMessagesAsync(string userId, string chatType, string chatId)
        {
            if (String.IsNullOrEmpty(chatId))
            {
                throw new BadRequestException("chatId gereklidir.");
            }

            var chat = await _chatRepository.GetChatByIdAsync(chatType, chatId) ?? throw new NotFoundException("Sohbet bulunamadı.");

            if (!chat.Participants.Contains(userId))
            {
                throw new ForbiddenException("Sohbet üzerinde yetkiniz yok.");
            }
            else if (chat.Messages == null || chat.Messages.Count == 0)
            {
                throw new NotFoundException("Mesaj bulunamadı.");

            }

            var messages = chat.Messages
                .Where(message => !message.Value.DeletedFor.ContainsKey(userId))
                .OrderBy(message => message.Value.Status.Sent)
                .ToDictionary(
                    message => message.Key,
                    message => message.Value
                );

            if (messages == null || messages.Any())
            {
                throw new NotFoundException("Mesaj bulunamadı.");
            }

            return messages;
        }


        public async Task<string> GetChatRecipientId(string userId, string chatType, string chatId)
        {
            if (String.IsNullOrEmpty(chatId))
            {
                throw new BadRequestException("chatId gereklidir.");
            }

            var chatParticipants = await _chatRepository.GetChatParticipantsAsync(chatType, chatId) ?? throw new NotFoundException("Sohbet bulunamadı.");

            if (chatType.Equals("Individual"))
            {
                if (!chatParticipants.Contains(userId))
                {
                    throw new ForbiddenException("Sohbet üzerinde yetkiniz yok.");
                }
            }
            else
            {
                throw new BadRequestException("chatType geçersiz.");
            }

            string recipientId = chatParticipants
                .FirstOrDefault(participant => !participant.Equals(userId))!;

            return recipientId;
        }


        public async Task<RecipientProfile> RecipientProfileAsync(string userId, string chatId)
        {
            if (String.IsNullOrEmpty(chatId))
            {
                throw new BadRequestException("chatId gereklidir.");
            }

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
    }
}