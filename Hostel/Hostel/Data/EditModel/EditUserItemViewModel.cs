using Suo.Admin.Data.ViewModel;

namespace Suo.Admin.Data.EditModel
{
    public class EditUserItemViewModel
    {        
        public bool IsConcurency { get; set; }               
        public UserViewModel Item { get; set; }
        public ClaimTemplateViewModel Template { get; set; }
        public List<UserViewModel> Models { get; set; }   
        
        public ModelSpec spec = new ModelSpec();        
        public string ConcurencyErrorText { get; set; }
    }
}
