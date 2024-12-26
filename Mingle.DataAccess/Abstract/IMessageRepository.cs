using Firebase.Database;
using Mingle.Entities.Models;

namespace Mingle.DataAccess.Abstract
{
    public interface IMessageRepository
    {
        Task CreateMessageAsync(string userId, string chatType, string chatId, string messageId, Message message);

        Task UpdateMessageDeletedForAsync(string chatType, string chatId, string messageId, Dictionary<string, DateTime> deletedFor);

        Task<IReadOnlyCollection<FirebaseObject<Message>>> GetMessagesByChatIdAsync(string chatType, string chatId);

        Task<Message> GetLastMessageByChatIdAsync(string chatType, string chatId);

        Task UpdateMessageStatusAsync(string chatType, string chatId, string messageId, string fieldName);

        Task<Message> GetMessageByIdAsync(string chatType, string chatId, string messageId);
    }
}