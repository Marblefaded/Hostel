
using HostelDB.Model;
using System.ComponentModel.DataAnnotations;

namespace Suo.Client.ViewModel
{
    public class UserViewModel : ICloneable
    {
        private User _item;
        public User Item => _item;
        public UserViewModel()
        {
            _item = new User();

        }
        public UserViewModel(User item)
        {
            _item = item;
        }

        [Key]
        public int UserId
        {
            get => _item.UserId;
            set => _item.UserId = value;
        }
        [Required(ErrorMessage = "Минимальное имя равняется двум буквам")]
        [MinLength(2)]
        public string Name
        {
            get => _item.Name;
            set => _item.Name = value;
        }
        [Required(ErrorMessage = "Минимальная фамилия равняется двум буквам")]
        [MinLength(2)]
        public string Surname
        {
            get => _item.Surname;
            set => _item.Surname = value;
        }
        //[Required(ErrorMessage = "Минимальное отчество равняется двум буквам")]
        //[MinLength(2)]
        public string Secondname
        {
            get => _item.Secondname;
            set => _item.Secondname = value;
        }
        public string PhoneNumber
        {
            get => _item.PhoneNumber;
            set => _item.PhoneNumber = value;
        }
              
        public string UserGroup
        {
            get => _item.UserGroup;
            set => _item.UserGroup = value;
        }
       
        public string UserCourse
        {
            get => _item.UserCourse;
            set => _item.UserCourse = value;
        }
                
        public string UserDeportament
        {
            get => _item.UserDeportament;
            set => _item.UserDeportament = value;
        }       

        ///Исключительно для процессинга таблиц
        public string? UserRoomNumber { get; set; }


        public object Clone()
        {
            UserViewModel tempObject = (UserViewModel)this.MemberwiseClone();
            tempObject._item = (User)_item.Clone();
            return tempObject;
        }
    }
}
