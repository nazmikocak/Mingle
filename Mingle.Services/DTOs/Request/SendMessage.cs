using Microsoft.AspNetCore.Http;
using Mingle.Entities.Enums;
using System.ComponentModel.DataAnnotations;

namespace Mingle.Services.DTOs.Request
{
    public sealed record SendMessage
    {
        [Required(ErrorMessage = "Lütfen bir içerik tipi seçiniz.")]
        [EnumDataType(typeof(MessageContent), ErrorMessage = "Geçersiz bir içerik seçildi.")]
        public MessageContent ContentType { get; init; }

        [MaxLength(2000, ErrorMessage = "Mesajınız en fazla 2000 karakter uzunluğunda olmalıdır.")]
        public string? TextContent { get; init; }

        public byte[]? FileContent { get; init; }
    }
}