using Suo.Admin.Data.ViewModel;

namespace Suo.Admin.Data.EditModel
{
    public class EditClaimItemViewModel
    {
        public bool IsOpened { get; set; }
        public bool IsConcurency { get; set; }
        public ClaimViewModel Item { get; set; }
        public UserViewModel ItemUser { get; set; }
        public List<ClaimViewModel> Models { get; set; }
        public string ConcurencyErrorText { get; set; }
    }
}
