using HostelDB.AlfaPruefungDb;
using HostelDB.DbRepository;
using Suo.Admin.Data.ViewModel;
using User = HostelDB.Model.User;

namespace Suo.Admin.Data.Service
{
    public class UserService
    {
        EFRepository<User> repoUser;
        private HostelDbContext DbContext;

        public UserService(HostelDbContext context)
        {
            repoUser = new EFRepository<User>(context);
            DbContext = context;
        }

        public List<UserViewModel> Get()
        {
            var list = repoUser.GetQuery().ToList();
            var result = list.Select(Convert).ToList();
            return result;
        }

        public static UserViewModel Convert(User r)
        {
            var item = new UserViewModel(r);
            return item;
        }

        public UserViewModel ReloadItem(UserViewModel item)
        {
            var x = repoUser.Reload(item.UserId);
            if (x == null)
            {
                return null;
            }
            return Convert(x);
        }

        public void Delete(UserViewModel item)
        {
            var x = repoUser.FindById(item.UserId);
            repoUser.Remove(x);
        }

        public UserViewModel Update(UserViewModel item)
        {
            var x = repoUser.FindById(item.UserId);
            x.Name = item.Name;
            x.Surname = item.Surname;
            x.Secondname = item.Secondname;           
            x.PhoneNumber = item.PhoneNumber;
            x.UserCourse = item.UserCourse;
            x.UserGroup = item.UserGroup;
            x.UserDeportament = item.UserDeportament;
            return Convert(repoUser.Update(x));
        }

        public UserViewModel Create(UserViewModel item)
        {
            var newItem = repoUser.Create(item.Item);
            return Convert(newItem);
        }

        public void Clear()
        {
            DbContext.ClearUser();
        }

        public string GetFIO(int userId)
        {
            var x = repoUser.FindById(userId);
            return x.Surname;
        }

        public List<UserViewModel> FilteringEmploers(string y)
        {
            y = y.Trim();
            var filteredListRooms = repoUser.GetQuery().Where(x => (x.Name.Contains(y) || x.Surname.Contains(y) || x.Secondname.Contains(y) || x.PhoneNumber.Contains(y) || x.UserDeportament.Contains(y) || x.UserCourse.Contains(y) || x.UserGroup.Contains(y))).ToList();
            var result = filteredListRooms.Select(Convert).ToList();
            return result;
        }

    }
}
