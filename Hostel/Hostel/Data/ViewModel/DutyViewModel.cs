using HostelDB.Model;

namespace Suo.Admin.Data.ViewModel
{
    public class DutyViewModel
    {
        private Duty _item;
        public Duty Item => _item;
        public DutyViewModel(Duty item)
        {
            _item = item;
        }
        public DutyViewModel()
        {
            _item = new Duty();
        }       
        public int DutyMonthId
        {
            get => _item.DutyMonthId;
            set => _item.DutyMonthId = value;
        }
        public DateTime Date
        {
            get => _item.Date;
            set => _item.Date = value;
        }
        public string RoomNumber
        {
            get => _item.RoomNumber;
            set => _item.RoomNumber = value;
        }
        public int RoomId
        {
            get;
            set;
        }
        public int Floor
        {
            get => _item.Floor;
            set => _item.Floor = value;
        }
        public string Wing
        {
            get => _item.Wing;
            set => _item.Wing = value;
        }

        public string? NotCompliteDuty
        {
            get => _item.NotCompliteDuty;
            set => _item.NotCompliteDuty = value;
        }
    }
}

