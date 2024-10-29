using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HostelDB.Model
{
    [Table("RoomType")]
    public class RoomType
    {
        [Key]
        public int RoomTypeId { get; set; }
        [Required, MaxLength(255)]        
        public string Title { get; set; }
        public virtual ICollection<Room> RoomCollection { get; set; }

        public RoomType() : base()
        {            
            RoomCollection = new HashSet<Room>();
        }
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
