using Mingle.Entities.Enums;
using Mingle.Entities.Models;

namespace Mingle.Services.Abstract
{
    /// <summary>
    /// Kullanıcılar arasındaki çağrı işlemleri için gerekli servis arayüzü.
    /// </summary>
    public interface ICallService
    {
        /// <summary>
        /// Bir çağrı başlatır.
        /// </summary>
        /// <param name="userId">Çağrıyı başlatan kullanıcının kimliği.</param>
        /// <param name="recipientId">Çağrının yapılacağı alıcı kullanıcının kimliği.</param>
        /// <param name="callType">Çağrı türü (sesli, görüntülü vb.).</param>
        /// <returns>Çağrı kimliğini içeren asenkron bir işlem döner.</returns>
        Task<string> StartCallAsync(string userId, string recipientId, CallType callType);



        /// <summary>
        /// Bir çağrıyı kabul eder.
        /// </summary>
        /// <param name="userId">Çağrıyı kabul eden kullanıcının kimliği.</param>
        /// <param name="callId">Kabul edilecek çağrının kimliği.</param>
        /// <returns>Asenkron bir işlem döner.</returns>
        Task AcceptCallAsync(string userId, string callId);



        /// <summary>
        /// Bir çağrıyı sonlandırır.
        /// </summary>
        /// <param name="userId">Çağrıyı sonlandıran kullanıcının kimliği.</param>
        /// <param name="callId">Sonlandırılacak çağrının kimliği.</param>
        /// <param name="callStatus">Çağrının durumu (başarılı, başarısız vb.).</param>
        /// <param name="createdDate">Çağrının oluşturulma tarihi.</param>
        /// <returns>Çağrı bilgilerini içeren bir sözlük döner.</returns>
        Task<Dictionary<string, Call>> EndCallAsync(string userId, string callId, CallStatus callStatus, DateTime? createdDate);



        /// <summary>
        /// Bir çağrıyı siler.
        /// </summary>
        /// <param name="userId">Çağrıyı silen kullanıcının kimliği.</param>
        /// <param name="callId">Silinecek çağrının kimliği.</param>
        /// <returns>Asenkron bir işlem döner.</returns>
        Task DeleteCallAsync(string userId, string callId);



        /// <summary>
        /// Bir çağrıya katılan kullanıcıları alır.
        /// </summary>
        /// <param name="userId">Kullanıcının kimliği.</param>
        /// <param name="callId">Katılımcıları alınacak çağrının kimliği.</param>
        /// <returns>Çağrıya katılan kullanıcıların kimliklerini içeren bir liste döner.</returns>
        Task<List<string>> GetCallParticipantsAsync(string userId, string callId);



        /// <summary>
        /// Kullanıcıya ait çağrı kayıtlarını alır.
        /// </summary>
        /// <param name="userId">Kullanıcının kimliği.</param>
        /// <returns>Çağrı günlüklerini içeren bir sözlük ve katılımcı listesi döner.</returns>
        Task<(Dictionary<string, Dictionary<string, Call>>, List<string>)> GetCallLogs(string userId);



        /// <summary>
        /// Belirli bir çağrı hakkında bilgi alır.
        /// </summary>
        /// <param name="userId">Çağrı bilgilerini alacak kullanıcının kimliği.</param>
        /// <param name="callId">Bilgileri alınacak çağrının kimliği.</param>
        /// <returns>Çağrı bilgilerini içeren bir çağrı nesnesi döner.</returns>
        Task<Call> GetCallAsync(string userId, string callId);
    }
}
