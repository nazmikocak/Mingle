using Firebase.Database;
using Mingle.Entities.Enums;
using Mingle.Entities.Models;

namespace Mingle.DataAccess.Abstract
{
    /// <summary>
    /// Gruplarla (Group) ilgili veritabanı işlemlerini tanımlayan arayüz.
    /// </summary>
    public interface IGroupRepository
    {
        /// <summary>
        /// Belirtilen grup kimliği ile yeni bir grup oluşturur veya mevcut grubu günceller.
        /// </summary>
        /// <param name="groupId">Grup kimliği.</param>
        /// <param name="group">Grup bilgilerini içeren nesne.</param>
        Task CreateOrUpdateGroupAsync(string groupId, Group group);



        /// <summary>
        /// Veritabanındaki tüm grupları getirir.
        /// </summary>
        /// <returns>Grup nesnelerinin koleksiyonu.</returns>
        Task<IReadOnlyCollection<FirebaseObject<Group>>> GetAllGroupAsync();



        /// <summary>
        /// Belirtilen grup kimliğine göre grubu getirir.
        /// </summary>
        /// <param name="groupId">Grup kimliği.</param>
        /// <returns>Grup bilgilerini içeren nesne.</returns>
        Task<Group> GetGroupByIdAsync(string groupId);



        /// <summary>
        /// Belirtilen grup kimliğine göre grup katılımcılarının kimliklerini getirir.
        /// </summary>
        /// <param name="groupId">Grup kimliği.</param>
        /// <returns>Katılımcıların kimliklerini içeren liste.</returns>
        Task<List<string>> GetGroupParticipantsIdsAsync(string groupId);



        /// <summary>
        /// Belirtilen grup kimliğine sahip grubun katılımcılarını günceller.
        /// </summary>
        /// <param name="groupId">Grup kimliği.</param>
        /// <param name="groupParticipants">Güncellenmiş grup katılımcıları listesi.</param>
        Task UpdateGroupParticipantsAsync(string groupId, Dictionary<string, GroupParticipant> groupParticipants);
    }
}