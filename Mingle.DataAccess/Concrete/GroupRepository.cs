//using Firebase.Database;
//using Firebase.Database.Query;
//using Mingle.DataAccess.Abstract;
//using Mingle.DataAccess.Configurations;
//using Mingle.Entities.Models;

//namespace Mingle.DataAccess.Concrete
//{
//    public class GroupRepository : IGroupRepository
//    {
//        private readonly FirebaseClient _databaseClient;


//        public GroupRepository(FirebaseConfig firebaseConfig)
//        {
//            _databaseClient = firebaseConfig.DatabaseClient;
//        }

//        public async Task CreateGroupAsync(string groupId, Group group)
//        {
//            await _databaseClient.Child("Groups").Child(groupId).PutAsync(group);
//        }
//    }
//}
