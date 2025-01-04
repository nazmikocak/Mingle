using Mingle.Entities.Models;
using Mingle.Services.DTOs.Request;

namespace Mingle.Services.Abstract
{
    public interface IMessageService
    {
        Task<(Dictionary<string, Dictionary<string, Dictionary<string, Message>>>, List<string>)> SendMessageAsync(string userId, string chatId, string chatType, SendMessage dto);

        Task<(Dictionary<string, Dictionary<string, Dictionary<string, Message>>>, List<string>)> DeleteMessageAsync(string userId, string chatType, string chatId, string messageId, byte deletionType);

        Task<(Dictionary<string, Dictionary<string, Dictionary<string, Message>>>, List<string>)> DeliverOrReadMessageAsync(string userId, string chatType, string chatId, string messageId, string fieldName);
    }
}