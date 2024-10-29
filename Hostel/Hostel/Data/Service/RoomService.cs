using HostelDB.AlfaPruefungDb;
using HostelDB.DbRepository;
using HostelDB.Model;
using NPOI.SS.Formula.Functions;
using Suo.Admin.Data.ViewModel;

namespace Suo.Admin.Data.Service
{
    public class RoomService
    {
        EFRepository<Room> _repo;
        private HostelDbContext _DbContext;

        public RoomService(HostelDbContext context)
        {
            _DbContext = context;
            _repo = new EFRepository<Room>(context);
        }

        public async Task<List<RoomViewModel>> GetRoomsFiltered(int floor, int type, string wing = "")
        {
            if (string.IsNullOrEmpty(wing))
            {
               return await Task.FromResult(_repo.GetQuery().Where(x => x.Floor == floor && x.RoomTypeId == type).ToList().Select(Convert).ToList());
            }
            else
            {
                return await Task.FromResult(_repo.GetQuery().Where(x => x.Floor == floor && x.RoomTypeId == type && x.Wing.Contains(wing)).ToList().Select(Convert).ToList());

            }
        }

        public List<RoomViewModel> Get()
        {
            var list = _repo.Get().ToList();
            var result = list.Select(Convert).ToList();
            return result;
        }
        public RoomViewModel Convert(Room rez)
        {
            var item = new RoomViewModel(rez);
            return item;
        }
        public RoomViewModel Create(RoomViewModel model)
        {
            var newitem = _repo.Create(model.Item);
            return Convert(newitem);
        }

        public RoomViewModel Update(RoomViewModel model)
        {
            var updateitem = _repo.Update(model.Item);
            return Convert(updateitem);
        }


        public RoomViewModel Remove(RoomViewModel model)
        {
            _repo.Remove(model.Item);
            return null;
        }


        public RoomViewModel FindById(int Id)
        {
            return Convert(_repo.FindById(Id));
        }

        public void Clear()
        {
            _DbContext.ClearRoom();
        }

        public async Task<List<DutyOrderViewModel>> GetRoomsWithOrders()
        {
            var query = (from room in _DbContext.DbSetRoom
                    join dutyOrder in _DbContext.DbSetDutyOrder on room.RoomId equals dutyOrder.RoomId
                    select new DutyOrderViewModel()
                    {
                        DutyOrderId = dutyOrder.DutyOrderId,
                        RoomId = room.RoomId,
                        Order = dutyOrder.Order,
                        Room = room,
                    }
                ).OrderBy(x=> x.Order).ToList();
            return await Task.FromResult(query.ToList());
        }
    }
}
