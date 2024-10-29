using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace HostelDB.Model
{
    [Table("User")]
    public class User
    {
        [Key]
        public int UserId { get; set; }
        [Required]
        [StringLength(255)]
        public string Name { get; set; }
        [Required]
        [StringLength(255)]
        public string Surname { get; set; }

        [StringLength(255)]
        public string Secondname { get; set; }
        
        [Required]
        [StringLength(255)]
        public string PhoneNumber { get; set; }
        [StringLength(50)]
        public string UserGroup { get; set; }
        [StringLength(50)]
        public string UserCourse { get; set; }
        [StringLength(255)]
        public string UserDeportament { get; set; }
        public virtual ICollection<TelegramUser> TelegramUserCollection { get; set; }
        public virtual ICollection<UserRoom> UserRoomCollection { get; set; }

        public User() : base()
        {
            TelegramUserCollection = new HashSet<TelegramUser>();
            UserRoomCollection = new HashSet<UserRoom>();
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
