using Mingle.Entities.Models;
using Mingle.Shared.DTOs.Request;

namespace Mingle.Services.Abstract
{
    /// <summary>
    /// Mesaj yönetim servislerini sağlayan arayüz.
    /// </summary>
    public interface IMessageService
    {
        /// <summary>
        /// Bir mesaj gönderir.
        /// </summary>
        /// <param name="userId">Mesajı gönderen kullanıcının kimliği.</param>
        /// <param name="chatId">Mesajın gönderileceği sohbetin kimliği.</param>
        /// <param name="chatType">Sohbet türü (örneğin bireysel, grup). </param>
        /// <param name="dto">Gönderilecek mesajın içeriğini içeren DTO.</param>
        /// <returns>Gönderilen mesajı ve ilgili bilgileri içeren bir sözlük döner.</returns>
        Task<(Dictionary<string, Dictionary<string, Dictionary<string, Message>>>, List<string>)> SendMessageAsync(string userId, string chatId, string chatType, SendMessage dto);



        /// <summary>
        /// Bir mesajı siler.
        /// </summary>
        /// <param name="userId">Mesajı silen kullanıcının kimliği.</param>
        /// <param name="chatType">Sohbet türü (örneğin bireysel, grup).</param>
        /// <param name="chatId">Mesajın silineceği sohbetin kimliği.</param>
        /// <param name="messageId">Silinecek mesajın kimliği.</param>
        /// <param name="deletionType">Mesajın silinme türü (örneğin, sadece kullanıcı tarafından mı, yoksa grup genelinde mi silinecek).</param>
        /// <returns>Silinen mesajla ilgili güncellenmiş sohbet bilgilerini içeren bir sözlük döner.</returns>
        Task<(Dictionary<string, Dictionary<string, Dictionary<string, Message>>>, List<string>)> DeleteMessageAsync(string userId, string chatType, string chatId, string messageId, byte deletionType);



        /// <summary>
        /// Bir mesajın teslim edildiğini veya okunduğunu işaretler.
        /// </summary>
        /// <param name="userId">Mesajı okuyan ya da teslim alan kullanıcının kimliği.</param>
        /// <param name="chatType">Sohbet türü (örneğin bireysel, grup).</param>
        /// <param name="chatId">Mesajın bulunduğu sohbetin kimliği.</param>
        /// <param name="messageId">İşaretlenecek mesajın kimliği.</param>
        /// <param name="fieldName">Hangi alanın (Okundu, Teslim Edildi vb.) güncellenmesi gerektiği.</param>
        /// <returns>Güncellenmiş sohbet bilgileri ile birlikte bir sözlük döner.</returns>
        Task<(Dictionary<string, Dictionary<string, Dictionary<string, Message>>>, List<string>)> DeliverOrReadMessageAsync(string userId, string chatType, string chatId, string messageId, string fieldName);
    }
}