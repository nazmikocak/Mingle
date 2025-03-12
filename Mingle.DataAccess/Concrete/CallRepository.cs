using Firebase.Database;
using Firebase.Database.Query;
using Microsoft.Extensions.Options;
using Mingle.DataAccess.Abstract;
using Mingle.DataAccess.Configurations;
using Mingle.Entities.Models;

namespace Mingle.DataAccess.Concrete
{
    /// <summary>
    /// Firebase veritabanında çağrı (Call) işlemlerini yöneten depo (repository) sınıfı.
    /// </summary>
    public sealed class CallRepository : ICallRepository
    {
        private readonly FirebaseClient _databaseClient;



        /// <summary>
        /// `CallRepository` sınıfını başlatır ve Firebase veritabanı bağlantısını yapılandırır.
        /// </summary>
        /// <param name="firebaseConfig">Firebase yapılandırma ayarlarını içeren nesne.</param>
        public CallRepository(FirebaseConfig firebaseConfig)
        {
            _databaseClient = firebaseConfig.DatabaseClient;
        }



        /// <summary>
        /// Firebase veritabanındaki tüm çağrıları getirir.
        /// </summary>
        /// <returns>Çağrı nesnelerinin koleksiyonu.</returns>
        public async Task<IReadOnlyCollection<FirebaseObject<Call>>> GetCallsAsync()
        {
            return await _databaseClient.Child("Calls").OnceAsync<Call>();
        }



        /// <summary>
        /// Belirtilen çağrıyı oluşturur veya mevcut bir çağrıyı günceller.
        /// </summary>
        /// <param name="callId">Çağrı kimliği.</param>
        /// <param name="call">Çağrı nesnesi.</param>
        public async Task CreateOrUpdateCallAsync(string callId, Call call)
        {
            await _databaseClient.Child("Calls").Child(callId).PutAsync(call);
        }



        /// <summary>
        /// Belirtilen kimliğe sahip çağrıyı getirir.
        /// </summary>
        /// <param name="callId">Çağrı kimliği.</param>
        /// <returns>Çağrı nesnesi.</returns>
        public async Task<Call> GetCallByIdAsync(string callId)
        {
            return await _databaseClient.Child("Calls").Child(callId).OnceSingleAsync<Call>();
        }



        /// <summary>
        /// Belirtilen çağrıya ait katılımcıların kimliklerini getirir.
        /// </summary>
        /// <param name="callId">Çağrı kimliği.</param>
        /// <returns>Katılımcıların kimliklerini içeren liste.</returns>
        public async Task<List<string>> GetCallParticipantsByIdAsync(string callId)
        {
            return await _databaseClient.Child("Calls").Child(callId).Child("Participants").OnceSingleAsync<List<string>>();
        }
    }
}