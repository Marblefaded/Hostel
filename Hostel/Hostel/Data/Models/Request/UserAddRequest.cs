using System.ComponentModel.DataAnnotations;
using Suo.Admin.Data.Models.Responce;

namespace Suo.Admin.Data.Models.Request;

public class UserAddRequest
{

    public UserAddRequest(UserResponse user)
    {
        this.Id = user.Id;
        this.UserName = user.UserName;
        this.FirstName = user.FirstName;
        this.LastName = user.LastName;
        this.Email = user.Email;
        this.IsActive = user.IsActive;
        this.EmailConfirmed = user.EmailConfirmed;
        this.PhoneNumber = user.PhoneNumber;
        this.LegacyId = user.LegacyId;
    }

    public UserAddRequest()
    {
        
    }
    public string Id { get; set; }
    public string UserName { get; set; }
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
    [Required]
    [EmailAddress(ErrorMessage = "Email is not Valid!")]
    public string Email { get; set; }
    public bool IsActive { get; set; }
    public bool EmailConfirmed { get; set; }
    public string PhoneNumber { get; set; }

    [Required]
    public bool IsAdmin { get; set; }
    [Required]
    [MinLength(8, ErrorMessage = "Minimum 8 characters")]
    public string Password { get; set; }

    public int LegacyId { get; set; }
}