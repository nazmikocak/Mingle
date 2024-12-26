using Mingle.Entities.Models;
using Mingle.Services.DTOs.Request;

namespace Mingle.Services.Abstract
{
    public interface IMessageService
    {
        Task<Dictionary<string, Message>> GetMessagesAsync(string userId, string chatId, string chatType);

        Task<Dictionary<string, Message>> SendMessageAsync(string userId, string chatId, string chatType, SendMessage dto);

        Task DeleteMessageAsync(string userId, string chatType, string chatId, string messageId, byte deletionType, Chat chat);

        Task<Message> DeliverOrReadMessageAsync(string userId, string chatType, string chatId, string messageId, string fieldName);
    }
}