using Microsoft.AspNetCore.Components;
using Suo.Admin.Data.EditModel;
using Suo.Admin.Data.ViewModel;

namespace Suo.Admin.Pages.Duty
{
    public class DutyForMonthUi : ComponentBase
    {
        [Parameter]
        public List<DateTime?> AllDaysList { get; set; }
        [Parameter]
        public EditDutyPageViewModel EditViewModel { get; set; }
        public DutyViewModel SelectDuty(DateTime? item, int floor, string wing)
        {
            var rez = EditViewModel.DutysForMonts.Where(x => x.Date == item && x.Floor == floor && x.Wing == wing).ToList();
            return rez[0];
        }
        public DutyViewModel SelectDutyFor3floor(DateTime? item, int floor)
        {
            var rez = EditViewModel.DutysForMonts.Where(x => x.Date == item && x.Floor == floor).ToList();
            return rez[0];
        }

    }
}
