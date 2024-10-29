using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Suo.Autorization.SingleService.Infrastructure.ResponseModels;

namespace Suo.Autorization.SingleService.Infrastructure.RequestModels;
public class UserUpdateRequest
{
    public UserUpdateRequest(UserResponse user)
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

    public UserUpdateRequest(UserAddRequest user)
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

    public UserUpdateRequest() { }

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
    public bool IsAdmin { get; set; }
    public string Password { get; set; }

    public int LegacyId { get; set; }
}
