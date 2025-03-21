using Mingle.Services.Exceptions;
using System.Text.RegularExpressions;

namespace Mingle.Services.Utilities
{
    /// <summary>
    /// Alan doğrulama işlemleri için yardımcı sınıf.
    /// </summary>
    internal static class FieldValidationHelper
    {
        /// <summary>
        /// Verilen alanların boş olup olmadığını kontrol eder ve boş olanlar için BadRequestException fırlatır.
        /// </summary>
        /// <param name="fields">Kontrol edilecek alanlar ve isimleri</param>
        /// <exception cref="BadRequestException">Eğer herhangi bir alan boşsa, exception fırlatılır.</exception>
        public static void ValidateRequiredFields(params (string Value, string FieldName)[] fields)
        {
            foreach (var (value, fieldName) in fields)
            {
                if (String.IsNullOrEmpty(value))
                {
                    throw new BadRequestException($"{fieldName} gereklidir.");
                }
            }
        }



        /// <summary>
        /// E-posta adresinin formatını doğrular. Geçerli formatta değilse BadRequestException fırlatır.
        /// </summary>
        /// <param name="email">Doğrulanacak e-posta adresi</param>
        /// <param name="fieldName">E-posta adresi alanının adı (varsayılan olarak "Email")</param>
        /// <exception cref="BadRequestException">Eğer e-posta formatı geçerli değilse, exception fırlatılır.</exception>
        public static void ValidateEmailFormat(string email, string fieldName = "Email")
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new BadRequestException($"{fieldName} gereklidir.");
            }

            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(email, emailPattern))
            {
                throw new BadRequestException($"{fieldName} geçerli bir e-posta adresi değil.");
            }
        }
    }
}