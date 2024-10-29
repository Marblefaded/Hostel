
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace HostelDB.Model
{
    [Table("Claim")]
    public class Claim
    {
        [Key]
        public int ClaimId { get; set; }
        public int ClaimTypeId { get; set; }
        public int Status { get; set; }
        [Required]
        public string ClaimJson { get; set; }
        [Required]
        public string ChangeLog { get; set; }
        public int ClaimTemplateId { get; set; }
        public int UserId { get; set; }
        public DateTime CreateDate { get; set; }
        public byte[] DataClaim { get; set; } 



        [Required, Timestamp]
        public byte[] RowVersion { get; set; }
        public object Clone()
        {
            return MemberwiseClone();
        }

        public virtual ClaimType ClaimType { get; set; }
        public virtual ClaimTemplate ClaimTemplate { get; set; }

       
    }
}
