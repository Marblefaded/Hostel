using System.ComponentModel.DataAnnotations;

namespace Suo.Autorization.SingleService.Infrastructure.RequestModels;

public class TokenRequest
{
    [Required]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
    public string UserDeviceUnfo { get; set; }
}