using Firebase.Database;
using Firebase.Database.Query;
using Microsoft.Extensions.Options;
using Mingle.DataAccess.Abstract;
using Mingle.DataAccess.Configurations;
using Mingle.Entities.Models;

namespace Mingle.DataAccess.Concrete
{
    /// <summary>
    /// Mesajlarla (Message) ilgili veritabanı işlemlerini gerçekleştiren repository sınıfı.
    /// </summary>
    public sealed class MessageRepository : IMessageRepository
    {
        private readonly FirebaseClient _databaseClient;



        /// <summary>
        /// <see cref="MessageRepository"/> sınıfının yeni bir örneğini başlatır.
        /// </summary>
        /// <param name="firebaseConfig">Firebase yapılandırma ayarlarını içeren nesne.</param>
        public MessageRepository(FirebaseConfig firebaseConfig)
        {
            _databaseClient = firebaseConfig.DatabaseClient;
        }



        /// <summary>
        /// Belirtilen kullanıcı kimliği, sohbet türü ve sohbet kimliğine göre yeni bir mesaj oluşturur.
        /// </summary>
        /// <param name="userId">Mesajı gönderen kullanıcının kimliği.</param>
        /// <param name="chatType">Sohbet türü (bireysel, grup vb.).</param>
        /// <param name="chatId">Sohbetin kimliği.</param>
        /// <param name="messageId">Mesajın kimliği.</param>
        /// <param name="message">Oluşturulacak mesaj nesnesi.</param>
        public async Task CreateMessageAsync(string userId, string chatType, string chatId, string messageId, Message message)
        {
            await _databaseClient.Child("Chats").Child(chatType).Child(chatId).Child("Messages").Child(messageId).PutAsync(message);
        }



        /// <summary>
        /// Belirtilen sohbet türü, sohbet kimliği ve mesaj kimliğine göre mesajın kimler için silindiğini günceller.
        /// </summary>
        /// <param name="chatType">Sohbet türü.</param>
        /// <param name="chatId">Sohbetin kimliği.</param>
        /// <param name="messageId">Mesajın kimliği.</param>
        /// <param name="deletedFor">Mesajı silen kullanıcıların kimlikleri ve silme zamanlarını içeren sözlük.</param>
        public async Task UpdateMessageDeletedForAsync(string chatType, string chatId, string messageId, Dictionary<string, DateTime> deletedFor)
        {
            await _databaseClient.Child("Chats").Child(chatType).Child(chatId).Child("Messages").Child(messageId).Child("DeletedFor").PutAsync(deletedFor);
        }



        /// <summary>
        /// Belirtilen sohbet türü, sohbet kimliği ve mesaj kimliğine göre mesajın durumunu günceller.
        /// </summary>
        /// <param name="chatType">Sohbet türü.</param>
        /// <param name="chatId">Sohbetin kimliği.</param>
        /// <param name="messageId">Mesajın kimliği.</param>
        /// <param name="fieldName">Güncellenecek durum alanının adı.</param>
        /// <param name="fieldData">Güncellenecek veri.</param>
        public async Task UpdateMessageStatusAsync(string chatType, string chatId, string messageId, string fieldName, object fieldData)
        {
            await _databaseClient.Child("Chats").Child(chatType).Child(chatId).Child("Messages").Child(messageId).Child("Status").Child(fieldName).PutAsync(fieldData);
        }
    }
}