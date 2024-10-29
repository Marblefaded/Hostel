using System.ComponentModel.DataAnnotations;

namespace Suo.Autorization.SingleService.Models
{
    public class UserChange
    {
        [Required(ErrorMessage = "Укажите пароль")]
        [StringLength(50, MinimumLength = 8, ErrorMessage = "Минимальная длина пароля 8 символов")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Пароли должны совпадать")]
        [Required(ErrorMessage = "Повторите пароль")]

        public string ConfirmPassword { get; set; }

        public string? Message { get; set; }
    }
}
