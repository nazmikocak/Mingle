using Firebase.Database;
using Firebase.Database.Query;
using Mingle.DataAccess.Abstract;
using Mingle.DataAccess.Configurations;
using Mingle.Entities.Enums;
using Mingle.Entities.Models;


namespace Mingle.DataAccess.Concrete
{
    public class GroupRepository : IGroupRepository
    {
        private readonly FirebaseClient _databaseClient;


        public GroupRepository(FirebaseConfig firebaseConfig)
        {
            _databaseClient = firebaseConfig.DatabaseClient;
        }


        public async Task CreateOrUpdateGroupAsync(string groupId, Group group)
        {
            await _databaseClient.Child("Groups").Child(groupId).PutAsync(group);
        }


        public async Task<IReadOnlyCollection<FirebaseObject<Group>>> GetAllGroupAsync()
        {
            return await _databaseClient.Child("Groups").OnceAsync<Group>();
        }


        public async Task<Group> GetGroupByIdAsync(string groupId)
        {
            return await _databaseClient.Child("Groups").Child(groupId).OnceSingleAsync<Group>();
        }


        public async Task UpdateGroupParticipantsAsync(string groupId, Dictionary<string, GroupParticipant> groupParticipants)
        {
            await _databaseClient.Child("Groups").Child(groupId).Child("Participants").PutAsync(groupParticipants);
        }
    }
}
