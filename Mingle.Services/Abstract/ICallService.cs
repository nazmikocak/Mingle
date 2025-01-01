using Mingle.Entities.Enums;
using Mingle.Entities.Models;

namespace Mingle.Services.Abstract
{
    public interface ICallService
    {
        Task<List<string>> StartCallAsync(string userId, string chatId, CallType callType);

        Task<Dictionary<string, Call>> EndCallAsync(string callId, CallStatus callStatus);

        Task<List<string>> GetCallParticipantsAsync(string userId, string callId);
    }
}