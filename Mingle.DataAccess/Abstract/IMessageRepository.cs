using Firebase.Database;
using Mingle.Entities.Models;

namespace Mingle.DataAccess.Abstract
{
    /// <summary>
    /// Mesajlarla (Message) ilgili veritabanı işlemlerini tanımlayan arayüz.
    /// </summary>
    public interface IMessageRepository
    {
        /// <summary>
        /// Belirtilen kullanıcı kimliği, sohbet türü ve sohbet kimliğine göre yeni bir mesaj oluşturur.
        /// </summary>
        /// <param name="userId">Mesajı gönderen kullanıcının kimliği.</param>
        /// <param name="chatType">Sohbet türü (bireysel, grup vb.).</param>
        /// <param name="chatId">Sohbetin kimliği.</param>
        /// <param name="messageId">Mesajın kimliği.</param>
        /// <param name="message">Oluşturulacak mesaj nesnesi.</param>
        Task CreateMessageAsync(string userId, string chatType, string chatId, string messageId, Message message);



        /// <summary>
        /// Belirtilen sohbet türü, sohbet kimliği ve mesaj kimliğine göre mesajın kimler için silindiğini günceller.
        /// </summary>
        /// <param name="chatType">Sohbet türü.</param>
        /// <param name="chatId">Sohbetin kimliği.</param>
        /// <param name="messageId">Mesajın kimliği.</param>
        /// <param name="deletedFor">Mesajı silen kullanıcıların kimlikleri ve silme zamanlarını içeren sözlük.</param>
        Task UpdateMessageDeletedForAsync(string chatType, string chatId, string messageId, Dictionary<string, DateTime> deletedFor);



        /// <summary>
        /// Belirtilen sohbet türü, sohbet kimliği ve mesaj kimliğine göre mesajın durumunu günceller.
        /// </summary>
        /// <param name="chatType">Sohbet türü.</param>
        /// <param name="chatId">Sohbetin kimliği.</param>
        /// <param name="messageId">Mesajın kimliği.</param>
        /// <param name="fieldName">Güncellenecek durum alanının adı.</param>
        /// <param name="fieldData">Güncellenecek veri.</param>
        Task UpdateMessageStatusAsync(string chatType, string chatId, string messageId, string fieldName, object fieldData);
    }
}