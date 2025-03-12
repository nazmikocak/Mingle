using Firebase.Database;
using Firebase.Database.Query;
using Mingle.DataAccess.Abstract;
using Mingle.DataAccess.Configurations;
using Mingle.Entities.Enums;
using Mingle.Entities.Models;


namespace Mingle.DataAccess.Concrete
{
    /// <summary>
    /// Firebase üzerinde grup (Group) yönetimi işlemlerini gerçekleştiren repository sınıfı.
    /// </summary>
    public sealed class GroupRepository : IGroupRepository
    {
        private readonly FirebaseClient _databaseClient;



        /// <summary>
        /// <see cref="GroupRepository"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        /// <param name="firebaseConfig">Firebase yapılandırma bilgilerini içeren nesne.</param>
        public GroupRepository(FirebaseConfig firebaseConfig)
        {
            _databaseClient = firebaseConfig.DatabaseClient;
        }



        /// <summary>
        /// Belirtilen grup kimliği ile yeni bir grup oluşturur veya mevcut grubu günceller.
        /// </summary>
        /// <param name="groupId">Grup kimliği.</param>
        /// <param name="group">Grup bilgilerini içeren nesne.</param>
        public async Task CreateOrUpdateGroupAsync(string groupId, Group group)
        {
            await _databaseClient.Child("Groups").Child(groupId).PutAsync(group);
        }



        /// <summary>
        /// Firebase veritabanındaki tüm grupları getirir.
        /// </summary>
        /// <returns>Grup nesnelerinin koleksiyonu.</returns>
        public async Task<IReadOnlyCollection<FirebaseObject<Group>>> GetAllGroupAsync()
        {
            return await _databaseClient.Child("Groups").OnceAsync<Group>();
        }



        /// <summary>
        /// Belirtilen grup kimliğine göre grubu getirir.
        /// </summary>
        /// <param name="groupId">Grup kimliği.</param>
        /// <returns>Grup bilgilerini içeren nesne.</returns>
        public async Task<Group> GetGroupByIdAsync(string groupId)
        {
            return await _databaseClient.Child("Groups").Child(groupId).OnceSingleAsync<Group>();
        }



        /// <summary>
        /// Belirtilen grup kimliğine göre grup katılımcılarının kimliklerini getirir.
        /// </summary>
        /// <param name="groupId">Grup kimliği.</param>
        /// <returns>Katılımcıların kimliklerini içeren liste.</returns>
        public async Task<List<string>> GetGroupParticipantsIdsAsync(string groupId)
        {
            var groupParticipants = await _databaseClient.Child("Groups").Child(groupId).Child("Participants").OnceAsync<object>();

            return groupParticipants
                .Where(x => !x.Object.Equals(GroupParticipant.Former))
                .Select(x => x.Key)
                .ToList();
        }



        /// <summary>
        /// Belirtilen grup kimliğine sahip grubun katılımcılarını günceller.
        /// </summary>
        /// <param name="groupId">Grup kimliği.</param>
        /// <param name="groupParticipants">Güncellenmiş grup katılımcıları listesi.</param>
        public async Task UpdateGroupParticipantsAsync(string groupId, Dictionary<string, GroupParticipant> groupParticipants)
        {
            await _databaseClient.Child("Groups").Child(groupId).Child("Participants").PutAsync(groupParticipants);
        }
    }
}