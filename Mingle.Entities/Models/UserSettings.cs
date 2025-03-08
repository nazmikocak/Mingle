using Mingle.Entities.Enums;

namespace Mingle.Entities.Models
{
    /// <summary>
    /// Kullanıcı ayarlarını temsil eden sınıf.
    /// Kullanıcının tema tercihi ve sohbet arka planı gibi kişisel ayarlarını içerir.
    /// </summary>
    public sealed class UserSettings
    {
        public Theme Theme { get; set; } = Enums.Theme.DefaultSystemMode;

        public string ChatBackground { get; set; } = "color1";
    }
}