using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace HostelDB.Model
{
    [Table("ClaimTemplate")]
    public class ClaimTemplate
    {

        [Key]
        public int ClaimTemplateId { get; set; }
        [Required, MaxLength(255)]
        public string Title { get; set; }
        [Required]
        public string ClaimJson { get; set; }
        public int ClaimTypeId { get; set; }
        public bool IsActive { get; set; }
        [Required]
        public string ChangeLog { get; set; }
        [Required]
        [Timestamp]
        public byte[] RowVersion { get; set; }
        public string TemplateModelJson { get; set; }
        public byte[] DataTemplate { get; set; }

        public virtual ClaimType ClaimType { get; set; }

        public virtual ICollection<Claim> ClaimCollection { get; set; }
        public ClaimTemplate() : base()
        {            
            ClaimCollection = new HashSet<Claim>();
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

    }
}
