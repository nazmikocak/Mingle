using Mingle.Entities.Enums;

namespace Mingle.Entities.Models
{
    public sealed class UserSettings
    {
        public Theme Theme { get; set; } = Enums.Theme.DefaultSystemMode;

        public string ChatBackground { get; set; } = "color1";
    }
}