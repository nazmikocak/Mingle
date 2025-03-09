using System.ComponentModel.DataAnnotations;

namespace Mingle.Shared.DTOs.Request
{
    /// <summary>
    /// Yapay zeka modeline gönderilecek isteği temsil eden veri transfer nesnesi (DTO).
    /// </summary>
    public sealed record AiRequest
    {
        [Required(ErrorMessage = "AI modeli seçilmelidir.")]
        public string AiModel { get; init; }


        [Required(ErrorMessage = "Lütfen bir propmt giriniz.")]
        [StringLength(1000, MinimumLength = 2, ErrorMessage = "Prompt en az 2, en fazla 1000 karakter olmalıdır.")]
        public string Prompt { get; init; }
    }
}