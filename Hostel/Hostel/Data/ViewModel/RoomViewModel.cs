using HostelDB.Model;

namespace Suo.Admin.Data.ViewModel
{
    public class RoomViewModel : ICloneable
    {
        private Room _item;
        public Room Item => _item;
        public RoomViewModel(Room item)
        {
            _item = item;
        }
        public RoomViewModel()
        {
            _item = new Room();
        }
        public int RoomId
        {
            get => _item.RoomId;
            set => _item.RoomId = value;
        }

        public string NumberRoom
        {
            get => _item.NumberRoom;
            set => _item.NumberRoom = value;
        }
        public string Wing
        {
            get => _item.Wing;
            set => _item.Wing = value;
        }
        public int RoomTypeId
        {
            get => _item.RoomTypeId;
            set => _item.RoomTypeId = value;
        }
        public int Floor
        {
            get => _item.Floor;
            set => _item.Floor = value;
        }
        public int PeopleMax
        {
            get => _item.PeopleMax;
            set => _item.PeopleMax = value;
        }
        public object Clone()
        {
            RoomViewModel item = (RoomViewModel)this.MemberwiseClone();
            item._item = (Room)_item.Clone();
            return item;
        }
    }
}


