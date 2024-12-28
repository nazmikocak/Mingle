using Mingle.Entities.Models;
using Mingle.Services.DTOs.Request;

namespace Mingle.Services.Abstract
{
    public interface IMessageService
    {
        Task<(Dictionary<string, Message>, string recipientId)> SendMessageAsync(string userId, string chatId, string chatType, SendMessage dto);

        Task DeleteMessageAsync(string userId, string chatType, string chatId, string messageId, byte deletionType);

        Task<(Dictionary<string, Message>, string recipientId)> DeliverOrReadMessageAsync(string userId, string chatType, string chatId, string messageId, string fieldName);
    }
}