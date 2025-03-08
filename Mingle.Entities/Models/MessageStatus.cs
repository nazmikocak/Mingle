namespace Mingle.Entities.Models
{
    /// <summary>
    /// Mesajın gönderim, teslim ve okunma durumlarını tutan sınıf.
    /// Her durum için ilgili tarih bilgilerini içerir.
    /// </summary>
    public sealed class MessageStatus
    {
        public required Dictionary<string, DateTime> Sent { get; set; }

        public Dictionary<string, DateTime> Delivered { get; set; } = [];

        public Dictionary<string, DateTime> Read { get; set; } = [];
    }
}