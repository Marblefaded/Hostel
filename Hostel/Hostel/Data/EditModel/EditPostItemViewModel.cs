using Suo.Admin.Data.ViewModel;

namespace Suo.Admin.Data.EditModel
{
    public class EditPostItemViewModel
    {       
        public bool IsConcurency { get; set; }
        public PostViewModel Item { get; set; }
        public List<PostViewModel> Models { get; set; }
        public string ConcurencyErrorText { get; set; }

        public List<string> ListSring { get; set; }

        public bool IsEdit;
    }
}
