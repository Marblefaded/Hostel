using HostelDB.Model;
namespace Suo.Admin.Data.ViewModel
{
    public class DutyOrderViewModel 
    {
        private DutyOrder _item;
        public DutyOrder Item => _item;
        public DutyOrderViewModel(DutyOrder item)
        {
            _item = item;
        }
        public DutyOrderViewModel()
        {
            _item = new DutyOrder();
        }
        public int DutyOrderId
        {
            get => _item.DutyOrderId;
            set => _item.DutyOrderId = value;
        }
      
        public int Order
        {
            get => _item.Order;
            set => _item.Order = value;
        }
        public int RoomId
        {
            get => _item.RoomId;
            set => _item.RoomId = value;
        }

        public Room Room { get; set; }
    }
}
