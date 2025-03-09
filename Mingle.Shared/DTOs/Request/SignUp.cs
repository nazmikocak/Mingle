using System.ComponentModel.DataAnnotations;

namespace Mingle.Shared.DTOs.Request
{
    /// <summary>
    /// Kullanıcıların sisteme kayıt olması için kullanılan veri transfer nesnesi (DTO).
    /// Ad, e-posta, şifre, doğum tarihi gibi bilgileri içerir.
    /// </summary>
    public sealed record SignUp
    {
        [Required(ErrorMessage = "Lütfen adınızı ve soyadınızı giriniz.")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Adınız ve soyadınız en az 5, en fazla 50 karakter uzunluğunda olmalıdır.")]
        [RegularExpression(@"^[A-Za-zÇçĞğİıÖöŞşÜü]+(?: [A-Za-zÇçĞğİıÖöŞşÜü]+)*$", ErrorMessage = "Adınız ve soyadınız yalnızca harflerden oluşmalı ve kelimeler arasında bir karakter boşluk olmalıdır.")]
        public string DisplayName { get; init; }


        [Required(ErrorMessage = "Lütfen e-mail adresinizi giriniz.")]
        [EmailAddress(ErrorMessage = "Lütfen geçerli bir e-mail adresi giriniz.")]
        [MaxLength(255, ErrorMessage = "E-Mail adresiniz en fazla 255 karakter uzunluğunda olmalıdır.")]
        public string Email { get; init; }


        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Lütfen şifrenizi giriniz.")]
        [StringLength(16, MinimumLength = 8, ErrorMessage = "Şifreniz en az 8, en fazla 16 karakter uzunluğunda olmalıdır.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)[A-Za-z\dÇçĞğİıÖöŞşÜü]*$", ErrorMessage = "Şifreniz en az bir büyük harf ve bir rakam içermelidir.")]
        public string Password { get; init; }


        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Lütfen şifrenizi tekrar giriniz.")]
        [Compare("Password", ErrorMessage = "Şifreler eşleşmiyor.")]
        public string PasswordAgain { get; init; }


        [Required(ErrorMessage = "Lütfen doğum tarihinizi giriniz.")]
        public DateTime BirthDate { get; init; }
    }
}