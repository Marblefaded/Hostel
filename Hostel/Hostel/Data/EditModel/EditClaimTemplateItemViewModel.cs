using NPOIPdfEngine.Models;
using Suo.Admin.Data.ViewModel;

namespace Suo.Admin.Data.EditModel
{
    public class EditClaimTemplateItemViewModel
    {       
        public bool IsConcurency { get; set; }
        public ClaimTemplateViewModel Item { get; set; }
        //public UserViewModel ItemUser { get; set; }
        //public List<ClaimTemplateViewModel> TemplateModels { get; set; }
        public string ConcurencyErrorText { get; set; }

        public ModelSpec spec = new ModelSpec();
        public ReportModel report { get; set; }
    }
}
