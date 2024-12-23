using Mingle.Entities.Models;

namespace Mingle.Services.Abstract
{
    public interface IMessageService
    {
        Task DeleteMessageAsync(string userId, string chatType, string chatId, string messageId, byte deletionType, Chat chat);
    }
}
