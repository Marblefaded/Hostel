using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HostelDB.Model
{
    [Table("DutyOrderList")]

    public class DutyOrder
    {
        [Key]
        public int DutyOrderId { get; set; }
        public int RoomId { get; set; }
        public int Order { get; set; }
        public virtual Room Room { get; set; }


        public object Clone()
        {
            return MemberwiseClone();
        }
    }

}

