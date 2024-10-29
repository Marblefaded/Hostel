using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HostelDB.Model
{
    [Table("Post")]
    public class Post
    {
        [Key]
        public int PostId { get; set; }
        [Required, MaxLength(255)]
        public string Title { get; set; }
        [Required]
        public string Text { get; set; }
        public string ListImageJson { get; set; }
        [Required]
        [Timestamp]
        public byte[] RowVersion { get; set; }
        [MaxLength(1000)]
        public string ChangeLog { get; set; }
        public DateTime CreateDate { get; set; }
        public object Clone()
        {
            return MemberwiseClone();
        }

    }
}
