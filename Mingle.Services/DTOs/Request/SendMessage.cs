using Microsoft.AspNetCore.Http;
using Mingle.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mingle.Services.DTOs.Request
{
    public sealed record SendMessage
    {
        [Required(ErrorMessage = "chatId gereklidir.")]
        public string ChatId { get; init; }

        [Required(ErrorMessage = "Lütfen bir içerik tipi seçiniz.")]
        [EnumDataType(typeof(MessageContent), ErrorMessage = "Geçersiz bir içerik seçildi.")]
        public MessageContent ContentType { get; init; }

        [MaxLength(2000, ErrorMessage = "Mesajınız en fazla 2000 karakter uzunluğunda olmalıdır.")]
        public string? TextContent { get; init; }

        public IFormFile? FileContent { get; init; }
    }
}