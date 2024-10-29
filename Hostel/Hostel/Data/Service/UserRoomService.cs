using HostelDB.AlfaPruefungDb;
using HostelDB.DbRepository;
using HostelDB.Model;
using Microsoft.AspNetCore.Components;
using Suo.Admin.Data.ViewModel;

namespace Suo.Admin.Data.Service
{
    public class UserRoomService
    {        
        EFRepository<UserRoom> _repo;
        EFRepository<User> _repoUser;
        EFRepository<Room> _repoRoom;
        private HostelDbContext _DbContext;

        public UserRoomService(HostelDbContext context)
        {
            _DbContext = context;
            _repo = new EFRepository<UserRoom>(context);
            _repoUser = new EFRepository<User>(context);
            _repoRoom = new EFRepository<Room>(context);
        }

        protected List<UserViewModel> UserModels { get; set; }

        public List<UserRoomViewModel> Get()
        {
            var list = _repo.Get().ToList();
            var result = list.Select(Convert).ToList();
            return result;
        }
        private UserRoomViewModel Convert(UserRoom rez)
        {
            var item = new UserRoomViewModel(rez);
            return item;
        }
        public UserRoomViewModel Remove(UserRoomViewModel model)
        {           
            _repo.Remove(model.Item);
            return null;
        }

        public UserRoomViewModel Create(UserRoomViewModel model)
        {                      
            var newitem = _repo.Create(model.Item);
            return Convert(newitem);
        }

        public UserRoomViewModel Update(UserRoomViewModel model)
        {
            var updateitem = _repo.Update(model.Item);
            return Convert(updateitem);
        }
       
        public UserRoomViewModel FindById(UserRoomViewModel item)
        {
            return Convert(_repo.FindById(item.UserId));
        }

        public List<UserViewModel> FilteringEmploers(string y)
        {
            var userIds = _repo.Get().ToList();
            var liveuserIds = userIds.Select(x => x.UserId).ToList();
            var filteredListRooms = _repoUser.GetQuery().Where(x => !liveuserIds.Contains(x.UserId)&& (x.Name.StartsWith(y) || x.Surname.StartsWith(y))).ToList();
            var result = filteredListRooms.Select(UserService.Convert).ToList();
            return result;          
        }

        public List<Room> Filtering(string y, int floor, int da) 
        {
            var userrooms = _repo.Get().ToList();
            var rooms = _repoRoom.Get().ToList();
            if (floor != -1)
            {
                rooms = _repoRoom.GetQuery().Where(x => x.Floor == floor).ToList();
            }
            if (da != -1)
            {
                rooms = _repoRoom.GetQuery().Where(x => x.RoomTypeId == da).ToList();
            }

            bool isNumber = int.TryParse(y, out int number);

            if (y == "")
            {              
                return rooms;
            }
            else if (isNumber == true)
            {
                var room = rooms.Where(x => x.NumberRoom.StartsWith(y)).ToList();
                //var roomIds = userrooms.Select(x => x.RoomId).ToList();
                //var filteredListRooms = userrooms.Where(x => roomIds.Contains(x.RoomId)).ToList();
                //var result = filteredListRooms.Select(Convert).ToList();
                return room;
            }
            else
            {
                var list = _repoUser.GetQuery().Where(x => x.Name.StartsWith(y) || x.Surname.StartsWith(y)).ToList();
                var userIds = list.Select(x => x.UserId).ToList();
                var filteredListUserRooms = userrooms.Where(x => userIds.Contains(x.UserId)).ToList();
                var roomIds = filteredListUserRooms.Select(x=>x.RoomId).ToList();
                var res = rooms.Where(x => roomIds.Contains(x.RoomId)).ToList();                
                return res;
            }
        }
        public void Clear()
        {
            _DbContext.ClearUserRoom();
        }
    }
}

