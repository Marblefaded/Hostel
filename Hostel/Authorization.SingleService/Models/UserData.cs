using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Suo.Autorization.SingleService.Models
{
    public class UserData
    {

        //[Remote(action: "VerifyEmail", controller: "ParameterToken")]
        [Required(ErrorMessage = "Укажите электронную почту")]
        [EmailAddress(ErrorMessage ="Вы ввели не корректную почту")]
        [DubleEmail]
        public string Email { get; set; }
        [Required(ErrorMessage = "Укажите пароль")]
        [StringLength(50, MinimumLength = 8, ErrorMessage = "Минимальная длина пароля 8 символов")]
        public string Password { get; set; }

        public string? UserDeviceUnfo { get; set; }

        [Compare("Password", ErrorMessage = "Пароли должны совпадать")]
        [Required(ErrorMessage = "Повторите пароль")]
        
        public string ConfirmPassword { get; set; }
        
        /*public string Message { get; set; } */
    }
}
