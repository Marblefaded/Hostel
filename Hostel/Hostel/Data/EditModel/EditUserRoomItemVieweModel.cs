using Suo.Admin.Data.ViewModel;

namespace Suo.Admin.Data.EditModel
{
    public class EditUserRoomItemVieweModel
    {
        public UserRoomViewModel UserRoomModel { get; set; }
        public RoomViewModel RoomModel { get; set; }

        public int RoomModelId { get; set; }

        public List<UserRoomViewModel> UserRoomModels { get; set; }
        public List<UserViewModel> UserModels { get; set; }
        public List<RoomViewModel> RoomModels { get; set; }
        public List<UserViewModel> UnsettledUserModels { get; set; }
        public bool maxPeople { get; set; }
    }
}
