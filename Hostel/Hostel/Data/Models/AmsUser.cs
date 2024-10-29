using Microsoft.AspNetCore.Identity;

namespace Suo.Admin.Data.Models;

public class AmsUser : IdentityUser<string>
{
    public string FirstName { get; set; }

    public string LastName { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }

    public string? LastModifiedBy { get; set; }

    public DateTime? LastModifiedOn { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedOn { get; set; }
    public bool IsActive { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
    //public int LegacyId { get; set; }
    public int UserId { get; set; }
    public AmsUser() : base()
    {

    }
}