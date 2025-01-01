using Mingle.Entities.Models;

namespace Mingle.Services.Abstract
{
    public interface IChatService
    {
        Task<Dictionary<string, Chat>> CreateChatAsync(string userId, string chatType, string recipientId);

        Task<(Dictionary<string, Dictionary<string, Chat>>, List<string>, List<string>)> GetAllChatsAsync(string userId);

        Task<Dictionary<string, Dictionary<string, Chat>>> ClearChatAsync(string userId, string chatType, string chatId);

        Task<Dictionary<string, Dictionary<string, Dictionary<string, DateTime>>>> ArchiveIndividualChatAsync(string userId, string chatId);

        Task<Dictionary<string, Dictionary<string, Dictionary<string, DateTime>>>> UnarchiveIndividualChatAsync(string userId, string chatId);
    }
}