using Microsoft.AspNetCore.Identity;

namespace Suo.Autorization.Data.Models;

public class AmsRole : IdentityRole
{
    public string Description { get; set; }
    public string CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public string LastModifiedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; }
    public virtual ICollection<AmsRoleClaim> RoleClaims { get; set; }

    public AmsRole() : base()
    {
        RoleClaims = new HashSet<AmsRoleClaim>();
    }

    public AmsRole(string roleName, string roleDescription = null) : base(roleName)
    {
        RoleClaims = new HashSet<AmsRoleClaim>();
        Description = roleDescription;
    }
}