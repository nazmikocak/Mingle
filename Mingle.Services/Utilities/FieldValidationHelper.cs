using Mingle.Services.Exceptions;
using System.Text.RegularExpressions;

namespace Mingle.Services.Utilities
{
    internal static class FieldValidationHelper
    {
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