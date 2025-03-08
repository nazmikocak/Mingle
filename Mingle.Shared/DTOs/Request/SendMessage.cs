using Mingle.Entities.Enums;
using System.ComponentModel.DataAnnotations;

namespace Mingle.Shared.DTOs.Request
{
    /// <summary>
    /// Kullanıcıların mesaj göndermesi için kullanılan veri transfer nesnesi (DTO).
    /// Mesaj içeriği ve içeriğin türünü içerir.
    /// </summary>
    public sealed record SendMessage
    {
        [Required(ErrorMessage = "Lütfen bir içerik tipi seçiniz.")]
        [EnumDataType(typeof(MessageContent), ErrorMessage = "Geçersiz bir içerik seçildi.")]
        public MessageContent ContentType { get; init; }

        [Required(ErrorMessage = "Lütfen bir mesaj giriniz.")]
        public string Content { get; init; }
    }
}