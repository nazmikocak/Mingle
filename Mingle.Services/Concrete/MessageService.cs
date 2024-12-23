using Mingle.DataAccess.Abstract;
using Mingle.Entities.Models;
using Mingle.Services.Abstract;
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



        public MessageService(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
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