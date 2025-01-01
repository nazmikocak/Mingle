using Firebase.Database;
using Firebase.Database.Query;
using Mingle.DataAccess.Abstract;
using Mingle.DataAccess.Configurations;
using Mingle.Entities.Models;

namespace Mingle.DataAccess.Concrete
{
    public class CallRepository : ICallRepository
    {
        private readonly FirebaseClient _databaseClient;


        public CallRepository(FirebaseConfig firebaseConfig)
        {
            _databaseClient = firebaseConfig.DatabaseClient;
        }


        public async Task CreateOrUpdateCallAsync(string callId, Call call)
        {
            await _databaseClient.Child("Calls").Child(callId).PutAsync(call);
        }


        public async Task<Call> GetCallByIdAsync(string callId)
        {
            return await _databaseClient.Child("Calls").Child(callId).OnceSingleAsync<Call>();
        }


        public async Task<List<string>> GetCallParticipantsByIdAsync(string callId)
        {
            return await _databaseClient.Child("Calls").Child(callId).Child("Participants").OnceSingleAsync<List<string>>();
        }
    }
}