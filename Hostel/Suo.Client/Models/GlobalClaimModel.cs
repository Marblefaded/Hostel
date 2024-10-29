using HostelDB.Model;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace Suo.Client.Models
{
    public class GlobalClaimModel
    {
        public ClaimTemplate claimTemplate;
        public Claim claim;
        public List<ClaimTemplate> claimTemplates { get; set; }
    }
}
