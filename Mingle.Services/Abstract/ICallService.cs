using Mingle.Entities.Enums;
using Mingle.Entities.Models;

namespace Mingle.Services.Abstract
{
    public interface ICallService
    {
        Task<string> StartCallAsync(string userId, string recipientId, CallType callType);

        Task<Dictionary<string, Call>> EndCallAsync(string userId, string callId, CallStatus callStatus, DateTime? createdDate);

        Task<List<string>> GetCallParticipantsAsync(string userId, string callId);

        Task<(Dictionary<string, Call>, List<string>)> GetCallLogs(string userId);
    }
}