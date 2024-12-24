using Firebase.Auth;
using Mingle.DataAccess.Abstract;
using Mingle.DataAccess.Concrete;
using Mingle.Entities.Models;
using Mingle.Services.Abstract;
using Mingle.Services.DTOs.Request;
using Mingle.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}