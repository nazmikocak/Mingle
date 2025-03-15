using Mingle.Services.Exceptions;

namespace Mingle.Services.Utilities
{
    internal static class FileValidationHelper
    {
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