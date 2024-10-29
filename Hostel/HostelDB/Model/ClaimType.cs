using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HostelDB.Model
{
    [Table("ClaimType")]
    public class ClaimType
    {
        [Key]
        public int ClaimTypeId { get; set; }
        [Required, MaxLength(255)]
        public string Title { get; set; }

        public virtual ICollection<ClaimTemplate> ClaimTemplateCollection { get; set; }
        public virtual ICollection<Claim> ClaimCollection { get; set; }
        public ClaimType() : base()
        {
            ClaimTemplateCollection = new HashSet<ClaimTemplate>();
            ClaimCollection = new HashSet<Claim>();
        }
    }
}
