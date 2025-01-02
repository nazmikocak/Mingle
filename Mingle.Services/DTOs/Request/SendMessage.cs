using Mingle.Entities.Enums;
using System.ComponentModel.DataAnnotations;

namespace Mingle.Services.DTOs.Request
{
    public sealed record SendMessage
    {
        [Required(ErrorMessage = "Lütfen bir içerik tipi seçiniz.")]
        [EnumDataType(typeof(MessageContent), ErrorMessage = "Geçersiz bir içerik seçildi.")]
        public MessageContent ContentType { get; init; }

        [Required(ErrorMessage = "Lütfen bir mesaj giriniz.")]
        public string Content { get; init; }
    }
}