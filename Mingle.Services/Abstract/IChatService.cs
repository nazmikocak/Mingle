using Mingle.Entities.Models;
using Mingle.Services.DTOs.Request;
using Mingle.Services.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mingle.Services.Abstract
{
    public interface IChatService
    {
        Task<string> CreateChatAsync(string userId, string chatType, string recipientId);

        Task ClearChatAsync(string userId, string chatType, string chatId);

        Task ArchiveIndividualChatAsync(string userId, string chatId);

        Task UnarchiveIndividualChatAsync(string userId, string chatId);

        Task<RecipientProfile> RecipientProfileAsync(string userId, string chatId);



        Task<string> GetChatRecipientId(string userId, string chatType, string chatId);






        Task SendMessageAsync(string userId, SendMessage dto);

        Task<Dictionary<string, Message>> GetMessagesAsync(string userId, string chatType, string chatId);










        
    }
}