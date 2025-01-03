using Microsoft.AspNetCore.Http;
using Mingle.Services.Exceptions;

namespace Mingle.Services.Utilities
{
    internal class FileValidationHelper
    {
        //public static void ValidatePhoto(IFormFile file, int maxFileSizeInMb, string[] allowedExtensions)
        //{
        //    const int BytesInMb = 1024 * 1024;
        //    int maxFileSize = maxFileSizeInMb * BytesInMb;

        //    if (file.Length > maxFileSize)
        //    {
        //        throw new BadRequestException($"Dosya boyutu en fazla {maxFileSizeInMb} MB olmalıdır.");
        //    }

        //    var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
        //    if (!allowedExtensions.Contains(fileExtension))
        //    {
        //        throw new BadRequestException($"Geçersiz dosya türü. Kabul edilen türler: {string.Join(", ", allowedExtensions)}");
        //    }
        //}


        public static void ValidatePhoto(MemoryStream file)
        {
            int maxFileSize = 2 * 1024 * 1024;

            if (file.Length > maxFileSize)
            {
                throw new BadRequestException($"Fotoğraf boyutu en fazla 2 MB olmalıdır.");
            }
        }


        public static void ValidateVideo(MemoryStream file)
        {
            int maxFileSize = 100 * 1024 * 1024;

            if (file.Length > maxFileSize)
            {
                throw new BadRequestException($"Video boyutu en fazla 100 MB olmalıdır.");
            }
        }


        public static void ValidateFile(MemoryStream file)
        {
            int maxFileSize = 200 * 1024 * 1024;

            if (file.Length > maxFileSize)
            {
                throw new BadRequestException($"Dosya boyutu en fazla 200 MB olmalıdır.");
            }
        }
    }
}