using Firebase.Database;
using Firebase.Database.Query;
using Microsoft.Extensions.Options;
using Mingle.DataAccess.Abstract;
using Mingle.DataAccess.Configurations;
using Mingle.Entities.Models;

namespace Mingle.DataAccess.Concrete
{
    /// <summary>
    /// Firebase veritabanında sohbet (Chat) işlemlerini yöneten repository (repository) sınıfı.
    /// </summary>
    public sealed class ChatRepository : IChatRepository
    {
        private readonly FirebaseClient _databaseClient;



        /// <summary>
        /// Firebase veritabanı istemcisi ile bağlantı kurar.
        /// </summary>
        /// <param name="firebaseConfig">Firebase yapılandırma ayarları.</param>
        public ChatRepository(FirebaseConfig firebaseConfig)
        {
            _databaseClient = firebaseConfig.DatabaseClient;
        }



        /// <summary>
        /// Belirtilen sohbet türüne göre tüm sohbetleri getirir.
        /// </summary>
        /// <param name="chatType">Sohbet türü (örneğin, grup, bireysel vb.).</param>
        /// <returns>Sohbet nesnelerinin koleksiyonu.</returns>
        public async Task<IReadOnlyCollection<FirebaseObject<Chat>>> GetChatsAsync(string chatType)
        {
            return await _databaseClient.Child("Chats").Child(chatType).OnceAsync<Chat>();
        }



        /// <summary>
        /// Yeni bir sohbet oluşturur veya mevcut sohbeti günceller.
        /// </summary>
        /// <param name="chatType">Sohbet türü.</param>
        /// <param name="chatId">Sohbet kimliği.</param>
        /// <param name="chat">Sohbet nesnesi.</param>
        public async Task CreateChatAsync(string chatType, string chatId, Chat chat)
        {
            await _databaseClient.Child("Chats").Child(chatType).Child(chatId).PutAsync(chat);
        }



        /// <summary>
        /// Belirtilen sohbet kimliği ve türüne göre sohbeti getirir.
        /// </summary>
        /// <param name="chatType">Sohbet türü.</param>
        /// <param name="chatId">Sohbet kimliği.</param>
        /// <returns>Sohbet nesnesi.</returns>
        public async Task<Chat> GetChatByIdAsync(string chatType, string chatId)
        {
            return await _databaseClient.Child("Chats").Child(chatType).Child(chatId).OnceSingleAsync<Chat>();
        }



        /// <summary>
        /// Belirtilen sohbetin katılımcılarını getirir.
        /// </summary>
        /// <param name="chatType">Sohbet türü.</param>
        /// <param name="chatId">Sohbet kimliği.</param>
        /// <returns>Katılımcıların kimliklerini içeren liste.</returns>
        public async Task<List<string>> GetChatParticipantsByIdAsync(string chatType, string chatId)
        {
            return await _databaseClient.Child("Chats").Child(chatType).Child(chatId).Child("Participants").OnceSingleAsync<List<string>>();
        }



        /// <summary>
        /// Belirtilen sohbetin arşivlenmiş kişiler bilgisini günceller.
        /// </summary>
        /// <param name="chatType">Sohbet türü.</param>
        /// <param name="chatId">Sohbet kimliği.</param>
        /// <param name="archivedFor">Arşivlenmiş kişilerin bilgilerinin yer aldığı sözlük.</param>
        public async Task UpdateChatArchivedForAsync(string chatType, string chatId, Dictionary<string, DateTime> archivedFor)
        {
            await _databaseClient.Child("Chats").Child(chatType).Child(chatId).PatchAsync(new { ArchivedFor = archivedFor });
        }



        /// <summary>
        /// Belirtilen sohbetin mesajlarını günceller.
        /// </summary>
        /// <param name="chatType">Sohbet türü.</param>
        /// <param name="chatId">Sohbet kimliği.</param>
        /// <param name="messages">Güncellenmiş mesajların yer aldığı sözlük.</param>
        public async Task UpdateChatMessageAsync(string chatType, string chatId, Dictionary<string, Message> messages)
        {
            await _databaseClient.Child("Chats").Child(chatType).Child(chatId).Child("Messages").PutAsync(messages);
        }
    }
}