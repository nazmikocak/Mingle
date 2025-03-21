using Mingle.Shared.DTOs.Request;
using Mingle.Shared.DTOs.Response;

namespace Mingle.Services.Abstract
{
    /// <summary>
    /// Grup yönetim servislerini sağlayan arayüz.
    /// </summary>
    public interface IGroupService
    {
        /// <summary>
        /// Yeni bir grup oluşturur.
        /// </summary>
        /// <param name="userId">Grup oluşturan kullanıcının kimliği.</param>
        /// <param name="dto">Grup oluşturma işlemi için gerekli verileri içeren DTO.</param>
        /// <returns>Oluşturulan grubun profil bilgilerini döner.</returns>
        Task<Dictionary<string, GroupProfile>> CreateGroupAsync(string userId, CreateGroup dto);



        /// <summary>
        /// Mevcut bir grubu düzenler.
        /// </summary>
        /// <param name="userId">Grubu düzenleyen kullanıcının kimliği.</param>
        /// <param name="groupId">Düzenlenecek grubun kimliği.</param>
        /// <param name="dto">Grup düzenleme işlemi için gerekli verileri içeren DTO.</param>
        /// <returns>Düzenlenen grubun profil bilgilerini döner.</returns>
        Task<Dictionary<string, GroupProfile>> EditGroupAsync(string userId, string groupId, CreateGroup dto);



        /// <summary>
        /// Bir grup için profil bilgilerini alır.
        /// </summary>
        /// <param name="userGroupIds">Kullanıcının üye olduğu grup kimlikleri.</param>
        /// <returns>Grupların profil bilgilerini içeren bir sözlük döner.</returns>
        Task<Dictionary<string, GroupProfile>> GetGroupProfilesAsync(List<string> userGroupIds);



        /// <summary>
        /// Bir grup için katılımcıların kimliklerini alır.
        /// </summary>
        /// <param name="userId">Grup katılımcılarını sorgulayan kullanıcının kimliği.</param>
        /// <param name="groupId">Katılımcıları sorgulanan grubun kimliği.</param>
        /// <returns>Grup katılımcılarının kimliklerini içeren bir liste döner.</returns>
        Task<List<string>> GetGroupParticipantsAsync(string userId, string groupId);



        /// <summary>
        /// Bir kullanıcıyı gruptan çıkarır.
        /// </summary>
        /// <param name="userId">Grup terk eden kullanıcının kimliği.</param>
        /// <param name="groupId">Terk edilen grubun kimliği.</param>
        /// <returns>Grup terk edildikten sonra güncellenmiş grup profil bilgilerini döner.</returns>
        Task<Dictionary<string, GroupProfile>> LeaveGroupAsync(string userId, string groupId);
    }
}