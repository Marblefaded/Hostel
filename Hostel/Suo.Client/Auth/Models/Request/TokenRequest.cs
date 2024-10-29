using System.ComponentModel.DataAnnotations;

namespace Suo.Client.Models.Request;

public class TokenRequest
{
    [Required]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
    public string UserDeviceUnfo { get; set; }
}