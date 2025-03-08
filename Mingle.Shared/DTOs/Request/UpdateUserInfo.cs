using Mingle.Entities.Enums;
using System.ComponentModel.DataAnnotations;

namespace Mingle.Shared.DTOs.Request
{
    /// <summary>
    /// Kullanıcıların profil fotoğrafını güncellemesi için kullanılan veri transfer nesnesi (DTO).
    /// </summary>
    public sealed record UpdateProfilePhoto
    {
        [Required(ErrorMessage = "Lütfen bir fotoğraf yükleyiniz.")]
        public string ProfilePhoto { get; init; }
    }



    /// <summary>
    /// Kullanıcıların ad ve soyad bilgisini güncellemesi için kullanılan veri transfer nesnesi (DTO).
    /// </summary>
    public sealed record UpdateDisplayName
    {
        [Required(ErrorMessage = "Lütfen adınızı ve soyadınızı giriniz.")]
        [MinLength(5, ErrorMessage = "Adınız ve soyadınız en az 5 karakter uzunluğunda olmalıdır.")]
        [MaxLength(50, ErrorMessage = "Adınız ve soyadınız en fazla 50 karakter uzunluğunda olmalıdır.")]
        [RegularExpression(@"^[A-Za-zÇçĞğİıÖöŞşÜü]+(?: [A-Za-zÇçĞğİıÖöŞşÜü]+)*$", ErrorMessage = "Adınız ve soyadınız yalnızca harflerden oluşmalı ve kelimeler arasında en fazla bir boşluk olmalıdır.")]
        public string DisplayName { get; init; }
    }



    /// <summary>
    /// Kullanıcıların telefon numarasını güncellemesi için kullanılan veri transfer nesnesi (DTO).
    /// </summary>
    public sealed record UpdatePhoneNumber
    {
        [Required(ErrorMessage = "Lütfen bir telefon numarası giriniz.")]
        // Regex
        public string PhoneNumber { get; init; }
    }



    /// <summary>
    /// Kullanıcıların biyografi bilgisini güncellemesi için kullanılan veri transfer nesnesi (DTO).
    /// </summary>
    public sealed record UpdateBiography
    {
        [Required(ErrorMessage = "Lütfen biyografinizi giriniz.")]
        [MinLength(1, ErrorMessage = "Biyografiniz en az 1 karakter uzunluğunda olmalıdır.")]
        [MaxLength(100, ErrorMessage = "Biyografiniz en fazla 100 karakter uzunluğunda olmalıdır.")]
        public string Biography { get; init; }
    }



    /// <summary>
    /// Kullanıcıların mevcut şifresini değiştirerek yeni bir şifre belirlemesi için kullanılan veri transfer nesnesi (DTO).
    /// </summary>
    public sealed record ChangePassword
    {
        [Required(ErrorMessage = "Lütfen mevcut şifrenizi giriniz.")]
        public string CurrentPassword { get; init; }

        [Required(ErrorMessage = "Lütfen yeni şifrenizi giriniz.")]
        [MinLength(8, ErrorMessage = "Şifreniz en az 8 karakter uzunluğunda olmalıdır.")]
        [MaxLength(16, ErrorMessage = "Şifreniz en fazla 16 karakter uzunluğunda olmalıdır.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)[A-Za-z\dÇçĞğİıÖöŞşÜü]*$", ErrorMessage = "Şifreniz en az bir büyük harf ve bir sayı içermelidir.")]
        public string NewPassword { get; init; }

        [Required(ErrorMessage = "Lütfen yeni şifrenizi tekrar giriniz.")]
        [Compare("NewPassword", ErrorMessage = "Şifreler eşleşmiyor.")]
        public string NewPasswordAgain { get; init; }
    }



    /// <summary>
    /// Kullanıcıların uygulama temasını değiştirmesi için kullanılan veri transfer nesnesi (DTO).
    /// </summary>
    public sealed record ChangeTheme
    {
        [Required(ErrorMessage = "Lütfen bir tema seçiniz.")]
        [EnumDataType(typeof(Theme), ErrorMessage = "Geçersiz bir tema seçildi.")]
        public Theme Theme { get; init; }
    }



    /// <summary>
    /// Kullanıcıların sohbet ekranının arka plan rengini değiştirmesi için kullanılan veri transfer nesnesi (DTO).
    /// </summary>
    public sealed record ChangeChatBackground
    {
        [Required(ErrorMessage = "Lütfen bir renk seçiniz.")]
        public string ChatBackground { get; init; }
    }
}