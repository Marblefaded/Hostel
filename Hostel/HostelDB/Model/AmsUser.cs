using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HostelDB.Model
{
    [Table("AspNetUsers")]
    public class AmsUser
    {
        [Key]
        public string Id { get; set; }
        public int? UserId { get; set; }       
    }
}
