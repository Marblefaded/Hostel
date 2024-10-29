using Microsoft.AspNetCore.Identity;

namespace Suo.Autorization.Data.Models;

public class AmsRoleClaim : IdentityRoleClaim<string>
{
    public string Description { get; set; }
    public string Group { get; set; }
    public string CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public string LastModifiedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; }
    public virtual AmsRole Role { get; set; }

    public AmsRoleClaim() : base()
    {
    }

    public AmsRoleClaim(string roleClaimDescription = null, string roleClaimGroup = null) : base()
    {

    }
}