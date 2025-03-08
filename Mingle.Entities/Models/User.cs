namespace Mingle.Entities.Models
{
    /// <summary>
    /// Kullanıcı bilgilerini temsil eden sınıf.
    /// Bir kullanıcının adı, e-posta, telefon numarası, biyografisi, profil fotoğrafı ve diğer kişisel bilgilerini içerir.
    /// </summary>
    public sealed class User
    {
        public required string DisplayName { get; set; }

        public required string Email { get; set; }

        public string PhoneNumber { get; set; } = string.Empty;

        public required string Biography { get; set; }

        public required Uri ProfilePhoto { get; set; }

        public required string ProviderId { get; set; }

        public DateTime LastConnectionDate { get; set; }

        public required DateTime BirthDate { get; set; }

        public required UserSettings UserSettings { get; set; }

        public required DateTime CreatedDate { get; set; }
    }
}