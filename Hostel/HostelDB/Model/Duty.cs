using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace HostelDB.Model
{
    [Table ("Duty")]
    public class Duty
    {
        [Key]
        public int DutyMonthId { get; set; }
        public DateTime Date { get; set; }
        [Required]
        [StringLength (250)]
        public string RoomNumber { get; set; }        
        [MaxLength(50)]
        public string? NotCompliteDuty { get; set; }
        public int Floor { get; set; }
        [Required]
        [StringLength(1)]
        public string Wing { get; set; }     
        
    }
}
