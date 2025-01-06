using System.ComponentModel.DataAnnotations;

namespace Mingle.Services.DTOs.Request
{
    public sealed record CreateGroup
    {
        [Required(ErrorMessage = "Lütfen bir grup ismi giriniz.")]
        [MinLength(2, ErrorMessage = "Grup ismi en az 2 karakter uzunluğunda olmalıdır.")]
        [MaxLength(50, ErrorMessage = "Grup ismi en fazla 50 karakter uzunluğunda olmalıdır.")]
        public string Name { get; init; }


        [Required(ErrorMessage = "Lütfen bir grup açıklaması giriniz.")]
        [MaxLength(100, ErrorMessage = "Grup açıklaması en fazla 100 karakter uzunluğunda olmalıdır.")]
        public string? Description { get; init; }


        public string? Photo { get; init; }


        public string? PhotoUrl { get; init; }


        [Required(ErrorMessage = "Grup oluşturabilmek için en az bir üye eklenmelidir.")]
        public string Participants { get; init; }
    }
}