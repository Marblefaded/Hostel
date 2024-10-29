using HostelDB.AlfaPruefungDb;
using HostelDB.DbRepository;
using HostelDB.Model;
using Suo.Admin.Data.ViewModel;

namespace Suo.Admin.Data.Service
{
    public class DutyOrderService
    {
        EFRepository<DutyOrder> _repo;
        private HostelDbContext _DbContext;

        public DutyOrderService(HostelDbContext context)
        {
            _DbContext = context;
            _repo = new EFRepository<DutyOrder>(context);
        }

        public List<DutyOrderViewModel> Get()
        {
            var list = _repo.Get().ToList();
            var result = list.Select(Convert).ToList();
            return result;
        }
        public DutyOrderViewModel Convert(DutyOrder rez)
        {
            var item = new DutyOrderViewModel(rez);
            return item;
        }
        public DutyOrderViewModel Create(DutyOrderViewModel model)
        {
            var newitem = _repo.Create(model.Item);
            return Convert(newitem);
        }

        public DutyOrderViewModel Update(DutyOrderViewModel model)
        {
            var updateitem = _repo.Update(model.Item);
            return Convert(updateitem);
        }


        public DutyOrderViewModel Remove(DutyOrderViewModel model)
        {
            _repo.Remove(model.Item);
            return null;
        }


        public DutyOrderViewModel FindById(int Id)
        {
            return Convert(_repo.FindById(Id));
        }

        public void Clear()
        {
            _DbContext.ClearOrder();
        }


        public void UpdateWithOrder(RoomViewModel item)
        {
            var newItem = new DutyOrder();
            newItem.RoomId = item.RoomId;
            newItem.Order = 0;
            _repo.Create(newItem);
        }

        public void RemoveWithOrder(RoomViewModel item)
        {
            var removingItem = _repo.GetQuery().Where(x => x.RoomId == item.RoomId).ToList();
            foreach (var dutyOrder in removingItem)
            {
                _repo.Remove(dutyOrder);
            }
        }
    }
}
