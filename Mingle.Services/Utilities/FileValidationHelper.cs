using Microsoft.AspNetCore.Http;
using Mingle.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mingle.Services.Utilities
{
    public class FileValidationHelper
    {
        public static void ValidatePhoto(IFormFile file, int maxFileSizeInMb, string[] allowedExtensions)
        {
            const int BytesInMb = 1024 * 1024;
            int maxFileSize = maxFileSizeInMb * BytesInMb;

            if (file.Length > maxFileSize)
            {
                throw new BadRequestException($"Dosya boyutu en fazla {maxFileSizeInMb} MB olmalıdır.");
            }

            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(fileExtension))
            {
                throw new BadRequestException($"Geçersiz dosya türü. Kabul edilen türler: {string.Join(", ", allowedExtensions)}");
            }
        }


        public static void ValidateProfilePhoto(IFormFile file)
        {
            int maxFileSize = 5 * 1024 * 1024;
            string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".svg", "webp " };

            if (file.Length > maxFileSize)
            {
                throw new BadRequestException($"Dosya boyutu en fazla 5 MB olmalıdır.");
            }

            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(fileExtension))
            {
                throw new BadRequestException($"Geçersiz dosya türü. Kabul edilen türler: {string.Join(", ", allowedExtensions)}");
            }
        }
    }
}
