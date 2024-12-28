using Firebase.Database;
using Mingle.Entities.Models;

namespace Mingle.DataAccess.Abstract
{
    public interface IChatRepository
    {
        Task CreateChatAsync(string chatType, string chatId, Chat chat);

        Task<IReadOnlyCollection<FirebaseObject<Chat>>> GetChatsAsync(string chatType);

        Task<Chat> GetChatByIdAsync(string chatType, string chatId);

        Task<List<string>> GetChatParticipantsAsync(string chatType, string chatId);

        Task UpdateChatArchivedForAsync(string chatType, string chatId, Dictionary<string, DateTime> archivedFor);

        Task UpdateChatMessageAsync(string chatType, string chatId, Dictionary<string, Message> messages);
    }
}