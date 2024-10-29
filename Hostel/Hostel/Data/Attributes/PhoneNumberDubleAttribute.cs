using System.ComponentModel.DataAnnotations;
using Suo.Admin.Data.Service;
using Suo.Admin.Data.ViewModel;

namespace Suo.Admin.Data.Attributes
{
    public class PhoneNumberDubleAttribute : ValidationAttribute
    {
        public string GetErrorMessage() =>
            $"В базе данных уже есть пользователь с таким номером телефона, введите другой.";

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            
            var service = (UserService)validationContext.GetService(typeof(UserService));


            var usermodel = validationContext.ObjectInstance;
            UserViewModel userViewModel = null;
            if (usermodel != null && usermodel is UserViewModel)
            {
                userViewModel = (UserViewModel)usermodel;
            }
            var userid = userViewModel.UserId;
            var users = service.Get();

            var result = users.FirstOrDefault(x => x.UserId == userid);

            if (result != null)
            {
                if (result.PhoneNumber == value.ToString())
                {
                    return ValidationResult.Success;
                }
            }

            var checkForDuble = users.FirstOrDefault(x => x.PhoneNumber == value.ToString());

            if (checkForDuble == null)
            {
                return ValidationResult.Success;
            }
            ///new List<string> { "PhoneNumber" } обнуляет кэш
            return new ValidationResult($"В базе данных уже есть пользователь с таким номером телефона, введите другой.", new List<string> { "PhoneNumber" });
        }
    }
}
