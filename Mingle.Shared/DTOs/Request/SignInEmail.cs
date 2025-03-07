using System.ComponentModel.DataAnnotations;

namespace Mingle.Shared.DTOs.Request
{
    public sealed record SignInEmail
    {
        [Required(ErrorMessage = "Lütfen e-posta adresinizi giriniz.")]
        [EmailAddress(ErrorMessage = "Lütfen geçerli bir e-posta adresi giriniz.")]
        public string Email { get; init; }


        [Required(ErrorMessage = "Lütfen şifrenizi giriniz.")]
        [DataType(DataType.Password)]
        public string Password { get; init; }
    }
}