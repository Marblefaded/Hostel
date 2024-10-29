using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HostelDB.Model
{
    [Table("Room")]
    public class Room
    {
        [Key]
        public int RoomId { get; set; }
        [Required]
        [StringLength(255)]
        public string NumberRoom { get; set; }
        [Required]
        [StringLength(1)]
        public string Wing { get; set; }
        public int RoomTypeId { get; set; }
        public int Floor { get; set; }
        public int PeopleMax { get; set; }
        [Required]
        [Timestamp]
        public byte[] RowVersion { get; set; }
        public virtual RoomType RoomType { get; set; }
        public virtual ICollection<UserRoom> UserRoomCollection { get; set; }
        public virtual ICollection<DutyOrder> DutyOrderCollection { get; set; }

        public Room() : base()
        {
            UserRoomCollection = new HashSet<UserRoom>();
        }


        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}

