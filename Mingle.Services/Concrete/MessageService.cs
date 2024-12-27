using Mingle.DataAccess.Abstract;
using Mingle.Entities.Models;
using Mingle.Services.Abstract;
using Mingle.Services.DTOs.Request;
using Mingle.Services.Exceptions;
using Mingle.Services.Utilities;

namespace Mingle.Services.Concrete
{
    public sealed class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly ICloudRepository _cloudRepository;
        private readonly IChatRepository _chatRepository;


        public MessageService(IMessageRepository messageRepository, ICloudRepository cloudRepository, IChatRepository chatRepository)
        {
            _messageRepository = messageRepository;
            _cloudRepository = cloudRepository;
            _chatRepository = chatRepository;
        }


        public async Task<Dictionary<string, Message>> GetMessagesAsync(string userId, string chatId, string chatType)
        {
            if (String.IsNullOrEmpty(chatId))
            {
                throw new BadRequestException("chatId gereklidir.");
            }

            var chat = await _chatRepository.GetChatByIdAsync(chatType, chatId) ?? throw new NotFoundException("Sohbet bulunamadı!");

            if (!chat.Participants.Contains(userId))
            {
                throw new ForbiddenException("Sohbet üzerinde yetkiniz yok.");
            }

            var messages = chat.Messages
                .Where(message => !message.Value.DeletedFor.ContainsKey(userId))
                .OrderBy(message => message.Value.Status.Sent)
                .ToDictionary(
                    message => message.Key,
                    message => message.Value
                );

            if (messages.Count == 0)
            {
                throw new NotFoundException("Mesaj bulunamadı.");
            }

            return messages;
        }


        public async Task<(Dictionary<string, Message>, string recipientId)> SendMessageAsync(string userId, string chatId, string chatType, SendMessage dto)
        {
            if (!(string.IsNullOrEmpty(dto.TextContent) ^ dto.FileContent == null))
            {
                throw new BadRequestException("TextContent veya FileContent gereklidir.");
            }

            // Katılımcıları ve dosya yükleme işlemini paralel başlat
            var participantsTask = _chatRepository.GetChatParticipantsAsync(chatType, chatId);
            Task<string>? fileUrlTask = null;

            if (dto.FileContent != null)
            {
                var photo = new MemoryStream(dto.FileContent);
                fileUrlTask = _cloudRepository.UploadPhotoAsync(userId, $"Chats/{chatId}", "image_message", photo)
                    .ContinueWith(task => task.Result.ToString());
            }

            // Katılımcıları kontrol et
            var chatParticipants = await participantsTask ?? throw new NotFoundException("Sohbet bulunamadı.");
            if (!chatParticipants.Contains(userId))
            {
                throw new ForbiddenException("Sohbet üzerinde yetkiniz yok.");
            }

            // Mesaj içeriği belirle
            string messageContent = dto.FileContent != null ? await fileUrlTask! : dto.TextContent;

            var recipientId = chatParticipants.SingleOrDefault(participant => !participant.Equals(userId));

            // Mesaj oluştur
            string messageId = Guid.NewGuid().ToString();
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

            return (new Dictionary<string, Message> { { messageId, message } }, recipientId);
        }



        public async Task DeleteMessageAsync(string userId, string chatType, string chatId, string messageId, byte deletionType, Chat chat)
        {
            if (String.IsNullOrEmpty(messageId))
            {
                throw new BadRequestException("messageId gereklidir.");
            }
            else if (deletionType.Equals(0) || deletionType.Equals(1))
            {
                throw new BadRequestException("deletionType geçersiz.");
            }

            if (!chat.Participants.Contains(userId))
            {
                throw new ForbiddenException("Sohbet üzerinde yetkiniz yok.");
            }
            else if (!chat.Messages.ContainsKey(messageId))
            {
                throw new NotFoundException("Mesaj bulunamadı.");
            }

            var deletedFor = chat.Messages.GetValueOrDefault(messageId).DeletedFor;

            if (deletionType.Equals(0))
            {
                if (!deletedFor.ContainsKey(userId))
                {
                    deletedFor.Add(userId, DateTime.UtcNow);
                }
                else
                {
                    throw new BadRequestException("Mesaj kullancı için zaten silinmiş.");
                }
            }
            else
            {
                foreach (var participant in chat.Participants)
                {
                    if (!deletedFor.ContainsKey(participant))
                    {
                        deletedFor.Add(participant, DateTime.UtcNow);
                    }
                }
            }

            await _messageRepository.UpdateMessageDeletedForAsync(chatType, chatId, messageId, deletedFor);
        }


        public async Task<Dictionary<string, Message>> DeliverOrReadMessageAsync(string userId, string chatType, string chatId, string messageId, string fieldName)
        {
            FieldValidator.ValidateRequiredFields(
                (chatType, "chatType"),
                (chatId, "chatId"),
                (messageId, "messageId")
            );

            var chat = await _chatRepository.GetChatByIdAsync(chatType, chatId) ?? throw new NotFoundException("Sohbet bulunamadı!");

            if (!chat.Participants.Contains(userId))
            {
                throw new ForbiddenException("Sohbet üzerinde yetkiniz yok.");
            }

            var message = chat.Messages.GetValueOrDefault(messageId) ?? throw new NotFoundException("Mesaj bulunamadı.");
            message.Status.Delivered.Add(userId, DateTime.UtcNow);

            return new Dictionary<string, Message> { { messageId, message } };
        }
    }
}