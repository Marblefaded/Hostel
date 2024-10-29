using AmsUser = HostelDB.Model.AmsUser;

namespace Suo.Admin.Data.ViewModel
{
    public class AmsUserViewModel
    {
        private AmsUser _item;
        public AmsUser Item => _item;
        public AmsUserViewModel(AmsUser item)
        {
            _item = item;
        }
        public AmsUserViewModel()
        {
            _item = new AmsUser();
        }
        public string Id
        {
            get => _item.Id;
            set => _item.Id = value;
        }
        public int? UserId
        {
            get => _item.UserId;
            set => _item.UserId = value;
        }
        
    }
}
