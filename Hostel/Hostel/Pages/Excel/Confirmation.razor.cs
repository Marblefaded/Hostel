using Microsoft.AspNetCore.Components;

namespace Suo.Admin.Pages.Excel
{
    public class ConfirmationView : ComponentBase
    {
        [Parameter] public EventCallback ClearTables { get; set; }

        public bool DialogIsOpen { get; set; }
        
    }
}
