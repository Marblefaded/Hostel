using Microsoft.AspNetCore.Components;
using MudBlazor;
using Suo.Admin.Data.ViewModel;

namespace Suo.Admin.Pages.LogApplication
{
    public class LogApplicationStackTraceView : ComponentBase
    {
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
        [Parameter] public LogApplicationViewModel LogViewModel { get; set; } = new LogApplicationViewModel();
        [Parameter] public string Title { get; set; }
    }
}
