using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HostelDB.Model
{
    [Table("UserRoom")]
    public class UserRoom : ICloneable
    {
        [Key]
        public int UserRoomId { get; set; }
        public int RoomId { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public virtual Room Room { get; set; }
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
