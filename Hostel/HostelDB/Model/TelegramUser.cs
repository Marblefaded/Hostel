using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HostelDB.Model
{
    [Table("TelegramUser")]
    public class TelegramUser
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        [Required]
        [StringLength(255)]
        public string TelegramUserId { get; set; }
        [Required]
        [StringLength(255)]
        public string TelegramUserFirstName { get; set; }
        
        [StringLength(255)]
        public string TelegramUserLastName { get; set; }
        public virtual User User { get; set; }
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
