using Mingle.Services.Exceptions;

namespace Mingle.Services.Utilities
{
    internal static class FieldValidator
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
    }
}