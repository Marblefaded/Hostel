using HostelDB.Model;
using Suo.Admin.Data.ViewModel;

namespace Suo.Admin.Data.EditModel
{
    public class EditDutyPageViewModel
    {
        public int InputYear { get; set; }
        public int InputFloor { get; set; }
        public int InputMonth { get; set; }
        public List<DutyViewModel> AllDutys { get; set; }
        
        public List<RoomViewModel> Rooms { get; set; }
        public List<UserRoomViewModel> UserRooms { get; set; }                                      
        public List<DutyOrderViewModel> DutyOrder { get; set; }                                      
        public DateTime Date { get; set; }
        public List<DutyViewModel> DutysForMonts = new List<DutyViewModel>();
        
        public List<DutyViewModel> DutysForPreviousMonts = new List<DutyViewModel>();
        public List<string> SlackerRooms = new List<string>();

        public List<RoomViewModel> ListRommsOn3FloorLeft { get; set; }
        public List<RoomViewModel> ListRommsOn4FloorLeft { get; set; }
        public List<RoomViewModel> ListRommsOn4FloorRight { get; set; }
        public List<RoomViewModel> ListRommsOn5FloorLeft { get; set; }
        public List<RoomViewModel> ListRommsOn5FloorRight { get; set; }


    }
}
