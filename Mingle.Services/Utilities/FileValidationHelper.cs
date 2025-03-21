using Mingle.Services.Exceptions;

namespace Mingle.Services.Utilities
{
    /// <summary>
    /// Dosya doğrulama işlemleri için yardımcı sınıf.
    /// </summary>
    internal static class FileValidationHelper
    {
        /// <summary>
        /// Fotoğraf dosyasını doğrular. Dosyanın boyutu 2 MB'dan fazla ise BadRequestException fırlatır.
        /// </summary>
        /// <param name="base64File">Base64 formatında fotoğraf verisi</param>
        /// <returns>Fotoğrafın bellek akışını döner.</returns>
        /// <exception cref="BadRequestException">Eğer fotoğraf boyutu 2 MB'dan fazla ise exception fırlatılır.</exception>
        public static MemoryStream ValidatePhoto(string base64File)
        {
            var photoBytes = Convert.FromBase64String(base64File);
            var photo = new MemoryStream(photoBytes);

            int maxFileSize = 2 * 1024 * 1024;

            if (photo.Length > maxFileSize)
            {
                throw new BadRequestException($"Fotoğraf boyutu en fazla 2 MB olmalıdır.");
            }

            return photo;
        }



        /// <summary>
        /// Video dosyasını doğrular. Dosyanın boyutu 100 MB'dan fazla ise BadRequestException fırlatır.
        /// </summary>
        /// <param name="base64File">Base64 formatında video verisi</param>
        /// <returns>Videonun bellek akışını döner.</returns>
        /// <exception cref="BadRequestException">Eğer video boyutu 100 MB'dan fazla ise exception fırlatılır.</exception>
        public static MemoryStream ValidateVideo(string base64File)
        {
            var videoBytes = Convert.FromBase64String(base64File);
            var video = new MemoryStream(videoBytes);

            int maxFileSize = 100 * 1024 * 1024;

            if (video.Length > maxFileSize)
            {
                throw new BadRequestException($"Video boyutu en fazla 100 MB olmalıdır.");
            }

            return video;
        }



        /// <summary>
        /// Genel dosya doğrulaması yapar. Dosyanın boyutu 200 MB'dan fazla ise BadRequestException fırlatır.
        /// </summary>
        /// <param name="base64File">Base64 formatında dosya verisi</param>
        /// <returns>Dosyanın bellek akışını döner.</returns>
        /// <exception cref="BadRequestException">Eğer dosya boyutu 200 MB'dan fazla ise exception fırlatılır.</exception>
        public static MemoryStream ValidateFile(string base64File)
        {
            var fileBytes = Convert.FromBase64String(base64File);
            var file = new MemoryStream(fileBytes);

            int maxFileSize = 200 * 1024 * 1024;

            if (file.Length > maxFileSize)
            {
                throw new BadRequestException($"Dosya boyutu en fazla 200 MB olmalıdır.");
            }

            return file;
        }
    }
}