using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;
using Suo.Autorization.SingleService.Infrastructure.Interfaces.IdentityServices;

namespace Suo.Autorization.SingleService.Models
{
    public class DubleEmailAttribute : ValidationAttribute
    {

        //public string Email { get; }

        public string GetErrorMessage() =>
            $"Пользователь с такой электронной почтой уже зарегестрирован";


        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var service = (IUserManagmentService)validationContext.GetService(typeof(IUserManagmentService));
            var x = false;
            if( value != null){
                x = service.CheckExistUserEmail((string)value).Result;
            }

            if (x == false)
            {
                return ValidationResult.Success;
            }
            return new ValidationResult(GetErrorMessage());
        }
    }

}
