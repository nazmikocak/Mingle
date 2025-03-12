using Firebase.Database;
using Firebase.Database.Query;
using Mingle.DataAccess.Abstract;
using Mingle.DataAccess.Configurations;
using Mingle.Entities.Models;

namespace Mingle.DataAccess.Concrete
{
    /// <summary>
    /// Firebase veritabanı ile kullanıcı (User) yönetimi işlemlerini gerçekleştiren repository sınıfıdır.
    /// </summary>
    public sealed class UserRepository : IUserRepository
    {
        private readonly FirebaseClient _databaseClient;



        /// <summary>
        /// UserRepository sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        /// <param name="firebaseConfig">Firebase yapılandırma bilgilerini içeren nesne.</param>
        public UserRepository(FirebaseConfig firebaseConfig)
        {
            _databaseClient = firebaseConfig.DatabaseClient;
        }



        /// <summary>
        /// Veritabanındaki tüm kullanıcıları getirir.
        /// </summary>
        /// <returns>Kullanıcıların listesini içeren bir koleksiyon.</returns>
        public async Task<IReadOnlyCollection<FirebaseObject<User>>> GetAllUsersAsync()
        {
            return await _databaseClient.Child("Users").OnceAsync<User>();
        }



        /// <summary>
        /// Yeni bir kullanıcı oluşturur veya mevcut kullanıcıyı günceller.
        /// </summary>
        /// <param name="userId">Kullanıcının benzersiz kimliği.</param>
        /// <param name="user">Kullanıcı bilgilerini içeren nesne.</param>
        public async Task CreateUserAsync(string userId, User user)
        {
            await _databaseClient.Child("Users").Child(userId).PatchAsync(user);
        }



        /// <summary>
        /// Belirtilen kimliğe sahip kullanıcıyı getirir.
        /// </summary>
        /// <param name="userId">Kullanıcının benzersiz kimliği.</param>
        /// <returns>Belirtilen kullanıcıya ait nesne.</returns>
        public async Task<User> GetUserByIdAsync(string userId)
        {
            return await _databaseClient.Child("Users").Child(userId).OnceSingleAsync<User>();
        }



        /// <summary>
        /// Kullanıcının belirli bir alanını günceller.
        /// </summary>
        /// <param name="userId">Kullanıcının benzersiz kimliği.</param>
        /// <param name="fieldName">Güncellenecek alanın adı.</param>
        /// <param name="newValue">Yeni değer.</param>
        public async Task UpdateUserFieldAsync(string userId, string fieldName, object newValue)
        {
            var fieldData = new Dictionary<string, object>
            {
                { fieldName, newValue }
            };

            await _databaseClient.Child("Users").Child(userId).PatchAsync(fieldData);
        }



        /// <summary>
        /// Kullanıcının belirli bir ayarını günceller.
        /// </summary>
        /// <param name="userId">Kullanıcının benzersiz kimliği.</param>
        /// <param name="settingsName">Güncellenecek ayar kategorisinin adı.</param>
        /// <param name="fieldName">Güncellenecek alanın adı.</param>
        /// <param name="newValue">Yeni değer.</param>
        public async Task UpdateSettingsAsync(string userId, string settingsName, string fieldName, object newValue)
        {
            var fieldData = new Dictionary<string, object>
            {
                { fieldName, newValue }
            };

            await _databaseClient.Child("Users").Child(userId).Child(settingsName).PatchAsync(fieldData);
        }
    }
}