using Sodium;
using System.Text;

namespace Mingle.Services.Utilities
{
    internal class EncryptionHelper
    {
        /// <summary>
        /// Asimetrik bir key çifti oluşturur (Public ve Private Key).
        /// </summary>
        /// <returns>Public ve Private Key çifti (Base64 olarak döndürülür).</returns>
        public static (string PublicKey, string PrivateKey) GenerateKeyPair()
        {
            // Libsodium ile key pair oluştur
            var keyPair = PublicKeyBox.GenerateKeyPair();

            // Key'leri Base64 formatında döndür
            return (
                Convert.ToBase64String(keyPair.PublicKey),
                Convert.ToBase64String(keyPair.PrivateKey)
            );
        }



        /// <summary>
        /// Verilen key'i chatId ile şifreler.
        /// </summary>
        /// <param name="key">Şifrelenecek key (Base64 formatında).</param>
        /// <param name="chatId">Şifreleme anahtarı olarak kullanılacak chatId.</param>
        /// <returns>Şifrelenmiş key (Base64 formatında).</returns>
        public static string EncryptKey(string key, string chatId)
        {
            // Key ve chatId'yi byte dizisine çevir
            var keyBytes = Convert.FromBase64String(key);
            var chatIdBytes = Encoding.UTF8.GetBytes(chatId);

            // Nonce oluştur (her işlem için benzersiz olmalı)
            var nonce = SecretBox.GenerateNonce();

            // Key'i chatId kullanarak şifrele
            var encryptedKey = SecretBox.Create(keyBytes, nonce, chatIdBytes);

            // Nonce ve şifrelenmiş key'i Base64 olarak döndür
            return $"{Convert.ToBase64String(nonce)}:{Convert.ToBase64String(encryptedKey)}";
        }
    }
}