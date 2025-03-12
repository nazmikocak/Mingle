using Firebase.Database;
using Mingle.Entities.Models;

namespace Mingle.DataAccess.Abstract
{
    /// <summary>
    /// Firebase veritabanında çağrı (Call) işlemlerini yöneten repository arayüzü.
    /// </summary>
    public interface ICallRepository
    {
        /// <summary>
        /// Firebase veritabanındaki tüm çağrıları getirir.
        /// </summary>
        /// <returns>Çağrı nesnelerinin koleksiyonu.</returns>
        Task<IReadOnlyCollection<FirebaseObject<Call>>> GetCallsAsync();



        /// <summary>
        /// Belirtilen çağrıyı oluşturur veya mevcut bir çağrıyı günceller.
        /// </summary>
        /// <param name="callId">Çağrı kimliği.</param>
        /// <param name="call">Çağrı nesnesi.</param>
        Task CreateOrUpdateCallAsync(string callId, Call call);



        /// <summary>
        /// Belirtilen kimliğe sahip çağrıyı getirir.
        /// </summary>
        /// <param name="callId">Çağrı kimliği.</param>
        /// <returns>Çağrı nesnesi.</returns>
        Task<Call> GetCallByIdAsync(string callId);



        /// <summary>
        /// Belirtilen çağrıya ait katılımcıların kimliklerini getirir.
        /// </summary>
        /// <param name="callId">Çağrı kimliği.</param>
        /// <returns>Katılımcıların kimliklerini içeren liste.</returns>
        Task<List<string>> GetCallParticipantsByIdAsync(string callId);
    }
}