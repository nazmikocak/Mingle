using System.ComponentModel.DataAnnotations;

namespace Mingle.Services.DTOs.Request
{
    public sealed record SearchedUsers
    {
        [Required(ErrorMessage = "Arama terimi boş olamaz.")]
        [MaxLength(50, ErrorMessage = "Arama terimi en fazla 50 karakter uzunluğunda olmalıdır.")]
        public string Query { get; init; }
    }
}