using HostelDB.Model;
using Suo.Admin.Data.ViewModel;

namespace Suo.Admin.Data.ViewModel
{
    public class UserRoomViewModel : ICloneable 
    { 
        private UserRoom _item;
        public UserRoom Item => _item;
        public UserRoomViewModel(UserRoom item)
        {
            _item = item;
        }
        public UserRoomViewModel()
        {
            _item = new UserRoom();
        }
        public int UserRoomId
        {
            get => _item.UserRoomId;
            set => _item.UserRoomId = value;
        }
        public int RoomId
        {
            get => _item.RoomId;
            set => _item.RoomId = value;
        }
        public int UserId
        {
            get => _item.UserId;
            set => _item.UserId = value;
        }
        public object Clone()
        {
            UserRoomViewModel item = (UserRoomViewModel)this.MemberwiseClone();
            item._item = (UserRoom)_item.Clone();
            return item;
        }
    }   
}


class UserRoomViewModelComparer : IEqualityComparer<UserRoomViewModel>
{
    // Products are equal if their names and product numbers are equal.
    public bool Equals(UserRoomViewModel x, UserRoomViewModel y)
    {

        //Check whether the compared objects reference the same data.
        if (Object.ReferenceEquals(x, y)) return true;

        //Check whether any of the compared objects is null.
        if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
            return false;

        //Check whether the products' properties are equal.
        return x.UserId == y.UserId && x.RoomId == y.RoomId;
    }

    // If Equals() returns true for a pair of objects
    // then GetHashCode() must return the same value for these objects.

    public int GetHashCode(UserRoomViewModel product)
    {
        //Check whether the object is null
        if (Object.ReferenceEquals(product, null)) return 0;

        //Get hash code for the Name field if it is not null.
        int hashProductName = product.UserId == null ? 0 : product.UserId.GetHashCode();

        //Get hash code for the Code field.
        int hashProductCode = product.RoomId.GetHashCode();

        //Calculate the hash code for the product.
        return hashProductName ^ hashProductCode;
    }
}