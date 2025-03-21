using Mingle.Entities.Models;

namespace Mingle.Services.Abstract
{
    /// <summary>
    /// Kullanıcılar arasındaki sohbet işlemleri için gerekli servis arayüzü.
    /// </summary>
    public interface IChatService
    {
        /// <summary>
        /// Yeni bir sohbet oluşturur.
        /// </summary>
        /// <param name="userId">Sohbeti başlatan kullanıcının kimliği.</param>
        /// <param name="chatType">Sohbet türü (örneğin: özel, grup). </param>
        /// <param name="recipientId">Sohbete katılacak alıcı kullanıcının kimliği.</param>
        /// <returns>Yeni oluşturulan sohbeti içeren bir sözlük döner.</returns>
        Task<Dictionary<string, Chat>> CreateChatAsync(string userId, string chatType, string recipientId);



        /// <summary>
        /// Kullanıcının tüm sohbetlerini getirir.
        /// </summary>
        /// <param name="userId">Kullanıcının kimliği.</param>
        /// <returns>Kullanıcının tüm sohbetleri, arşivlenmiş ve silinmiş sohbetler hakkında bilgi döner.</returns>
        Task<(Dictionary<string, Dictionary<string, Chat>>, List<string>, List<string>)> GetAllChatsAsync(string userId);



        /// <summary>
        /// Bir sohbeti temizler.
        /// </summary>
        /// <param name="userId">Sohbeti temizleyecek kullanıcının kimliği.</param>
        /// <param name="chatType">Temizlenecek sohbetin türü.</param>
        /// <param name="chatId">Temizlenecek sohbetin kimliği.</param>
        /// <returns>Temizlenmiş sohbeti içeren bir sözlük döner.</returns>
        Task<Dictionary<string, Dictionary<string, Chat>>> ClearChatAsync(string userId, string chatType, string chatId);



        /// <summary>
        /// Bireysel bir sohbeti arşivler.
        /// </summary>
        /// <param name="userId">Sohbeti arşivleyecek kullanıcının kimliği.</param>
        /// <param name="chatId">Arşivlenecek sohbetin kimliği.</param>
        /// <returns>Arşivlenmiş sohbet bilgilerini içeren bir sözlük döner.</returns>
        Task<Dictionary<string, Dictionary<string, Dictionary<string, DateTime>>>> ArchiveIndividualChatAsync(string userId, string chatId);
        


        /// <summary>
        /// Bireysel bir sohbeti arşivden çıkarır.
        /// </summary>
        /// <param name="userId">Sohbeti arşivden çıkaracak kullanıcının kimliği.</param>
        /// <param name="chatId">Arşivden çıkarılacak sohbetin kimliği.</param>
        /// <returns>Arşivden çıkarılmış sohbet bilgilerini içeren bir sözlük döner.</returns>
        Task<Dictionary<string, Dictionary<string, Dictionary<string, DateTime>>>> UnarchiveIndividualChatAsync(string userId, string chatId);
    }
}