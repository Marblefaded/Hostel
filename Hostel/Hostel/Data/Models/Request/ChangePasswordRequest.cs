using System.ComponentModel.DataAnnotations;

namespace Suo.Admin.Data.Models.Request;

public class ChangePasswordRequest
{
    [Required]
    public string Password { get; set; }

    [Required]
    public string NewPassword { get; set; }

    [Required]
    public string ConfirmNewPassword { get; set; }
}