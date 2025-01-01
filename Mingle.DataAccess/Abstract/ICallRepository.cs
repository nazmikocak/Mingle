using Mingle.Entities.Models;

namespace Mingle.DataAccess.Abstract
{
    public interface ICallRepository
    {
        Task CreateOrUpdateCallAsync(string callId, Call call);

        Task<Call> GetCallByIdAsync(string callId);

        Task<List<string>> GetCallParticipantsByIdAsync(string callId);
    }
}