using Firebase.Database;
using Mingle.Entities.Models;

namespace Mingle.DataAccess.Abstract
{
    public interface ICallRepository
    {
        Task<IReadOnlyCollection<FirebaseObject<Call>>> GetCallsAsync();

        Task CreateOrUpdateCallAsync(string callId, Call call);

        Task<Call> GetCallByIdAsync(string callId);

        Task<List<string>> GetCallParticipantsByIdAsync(string callId);
    }
}