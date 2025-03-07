using System.ComponentModel.DataAnnotations;

namespace Mingle.Shared.DTOs.Request
{
    public sealed record SignUp
    {
        [Required(ErrorMessage = "Lütfen adınızı ve soyadınızı giriniz.")]
        [MinLength(5, ErrorMessage = "Adınız ve soyadınız en az 5 karakter uzunluğunda olmalıdır.")]
        [MaxLength(50, ErrorMessage = "Adınız ve soyadınız en fazla 50 karakter uzunluğunda olmalıdır.")]
        [RegularExpression(@"^[A-Za-zÇçĞğİıÖöŞşÜü]+(?: [A-Za-zÇçĞğİıÖöŞşÜü]+)*$", ErrorMessage = "Adınız ve soyadınız yalnızca harflerden oluşmalı ve kelimeler arasında en fazla bir boşluk olmalıdır.")]
        public string DisplayName { get; init; }


        [Required(ErrorMessage = "Lütfen e-mail adresinizi giriniz.")]
        [EmailAddress(ErrorMessage = "Lütfen geçerli bir e-mail adresi giriniz.")]
        [MaxLength(30, ErrorMessage = "E-Mail adresiniz en fazla 30 karakter uzunluğunda olmalıdır.")]
        public string Email { get; init; }


        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Lütfen şifrenizi giriniz.")]
        [MinLength(8, ErrorMessage = "Şifreniz en az 8 karakter uzunluğunda olmalıdır.")]
        [MaxLength(16, ErrorMessage = "Şifreniz en fazla 16 karakter uzunluğunda olmalıdır.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)[A-Za-z\dÇçĞğİıÖöŞşÜü]*$", ErrorMessage = "Şifreniz en az bir büyük harf ve bir sayı içermelidir.")]
        public string Password { get; init; }


        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Lütfen şifrenizi tekrar giriniz.")]
        [Compare("Password", ErrorMessage = "Şifreler eşleşmiyor.")]
        public string PasswordAgain { get; init; }


        [Required(ErrorMessage = "Lütfen doğum tarihinizi giriniz.")]
        public DateTime BirthDate { get; init; }
    }
}