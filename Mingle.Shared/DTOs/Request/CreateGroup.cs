using System.ComponentModel.DataAnnotations;

namespace Mingle.Shared.DTOs.Request
{
    /// <summary>
    /// Yeni bir grup oluşturmak için kullanılan veri transfer nesnesi (DTO).
    /// Grup ismi, açıklaması, fotoğraf bilgisi ve katılımcıları içerir.
    /// </summary>
    public sealed record CreateGroup
    {
        [Required(ErrorMessage = "Lütfen bir grup adı giriniz.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Grup ismi en az 2, en fazla 50 karakter uzunluğunda olmalıdır.")]
        public string Name { get; init; }


        [MaxLength(100, ErrorMessage = "Grup açıklaması en fazla 100 karakter uzunluğunda olmalıdır.")]
        public string? Description { get; init; }


        public string? Photo { get; init; }


        public string? PhotoUrl { get; init; }


        [Required(ErrorMessage = "Grup oluşturabilmek için en az bir üye eklenmelidir.")]
        public string Participants { get; init; }
    }
}