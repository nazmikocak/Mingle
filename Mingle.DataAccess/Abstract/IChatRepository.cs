using Firebase.Database;
using Mingle.Entities.Models;

namespace Mingle.DataAccess.Abstract
{
    /// <summary>
    /// Sohbet (Chat) işlemlerini yöneten repository arayüzü.
    /// </summary>
    public interface IChatRepository
    {
        /// <summary>
        /// Yeni bir sohbet oluşturur veya mevcut sohbeti günceller.
        /// </summary>
        /// <param name="chatType">Sohbet türü (örneğin, grup, bireysel vb.).</param>
        /// <param name="chatId">Sohbet kimliği.</param>
        /// <param name="chat">Sohbet nesnesi.</param>
        Task CreateChatAsync(string chatType, string chatId, Chat chat);



        /// <summary>
        /// Belirtilen sohbet türüne göre tüm sohbetleri getirir.
        /// </summary>
        /// <param name="chatType">Sohbet türü.</param>
        /// <returns>Sohbet nesnelerinin koleksiyonu.</returns>
        Task<IReadOnlyCollection<FirebaseObject<Chat>>> GetChatsAsync(string chatType);



        /// <summary>
        /// Belirtilen sohbet kimliği ve türüne göre sohbeti getirir.
        /// </summary>
        /// <param name="chatType">Sohbet türü.</param>
        /// <param name="chatId">Sohbet kimliği.</param>
        /// <returns>Sohbet nesnesi.</returns>
        Task<Chat> GetChatByIdAsync(string chatType, string chatId);



        /// <summary>
        /// Belirtilen sohbetin katılımcılarını getirir.
        /// </summary>
        /// <param name="chatType">Sohbet türü.</param>
        /// <param name="chatId">Sohbet kimliği.</param>
        /// <returns>Katılımcıların kimliklerini içeren liste.</returns>
        Task<List<string>> GetChatParticipantsByIdAsync(string chatType, string chatId);



        /// <summary>
        /// Belirtilen sohbetin arşivlenmiş kişiler bilgisini günceller.
        /// </summary>
        /// <param name="chatType">Sohbet türü.</param>
        /// <param name="chatId">Sohbet kimliği.</param>
        /// <param name="archivedFor">Arşivlenmiş kişilerin bilgilerinin yer aldığı sözlük.</param>
        Task UpdateChatArchivedForAsync(string chatType, string chatId, Dictionary<string, DateTime> archivedFor);



        /// <summary>
        /// Belirtilen sohbetin mesajlarını günceller.
        /// </summary>
        /// <param name="chatType">Sohbet türü.</param>
        /// <param name="chatId">Sohbet kimliği.</param>
        /// <param name="messages">Güncellenmiş mesajların yer aldığı sözlük.</param>
        Task UpdateChatMessageAsync(string chatType, string chatId, Dictionary<string, Message> messages);
    }
}