using Mingle.Entities.Models;

namespace Mingle.Services.Abstract
{
    public interface IChatService
    {
        Task<Dictionary<string, Chat>> CreateChatAsync(string userId, string chatType, string recipientId);

        Task<(Dictionary<string, Dictionary<string, Chat>>, List<string>, List<string>, List<string>)> GetAllChatsAsync(string userId);

        Task<Dictionary<string, Chat>> ClearChatAsync(string userId, string chatType, string chatId);

        Task ArchiveIndividualChatAsync(string userId, string chatId);

        Task UnarchiveIndividualChatAsync(string userId, string chatId);
    }
}