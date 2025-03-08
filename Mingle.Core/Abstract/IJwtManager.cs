namespace Mingle.Core.Abstract
{
    /// <summary>
    /// JSON Web Token (JWT) oluşturma işlemi için bir sözleşme sağlayan arayüz.
    /// </summary>
    public interface IJwtManager
    {
        /// <summary>
        /// Verilen kullanıcı kimliği için bir JWT oluşturur.
        /// </summary>
        /// <param name="userId">JWT'yi oluşturmak için kullanılacak kullanıcı kimliği.</param>
        /// <returns>Oluşturulan JWT'yi temsil eden bir dize döner.</returns>
        /// <exception cref="ArgumentNullException">Geçersiz veya eksik yapılandırma verisi durumunda fırlatılır.</exception>
        string GenerateToken(string userId);
    }
}