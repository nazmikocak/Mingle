using Mingle.Entities.Models;

namespace Mingle.Services.Abstract
{
    public interface IChatService
    {
        Task<Dictionary<string, Chat>> CreateChatAsync(string userId, string chatType, string recipientId);

        Task ClearChatAsync(string userId, string chatType, string chatId);

        Task ArchiveIndividualChatAsync(string userId, string chatId);

        Task UnarchiveIndividualChatAsync(string userId, string chatId);

        Task<string> GetChatRecipientIdAsync(string userId, string chatType, string chatId);

        Task<(Dictionary<string, Dictionary<string, Chat>>, List<string>, List<string>)> GetChatsAsync(string userId);
    }
}