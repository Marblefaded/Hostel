using Microsoft.AspNetCore.Components;
using MudBlazor;
using Suo.Admin.Data.Service;
using Suo.Admin.Data.ViewModel;

namespace Suo.Admin.Pages.User
{
    public class EditUserView : ComponentBase
    {
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
        [Parameter]
        public UserViewModel UserViewModel { get; set; } = new UserViewModel();
        [Parameter]
        public string Title { get; set; }
        [Inject] protected UserService Service { get; set; }
        public void Cancel()
        {
            MudDialog.Cancel();
        }
        public void Save()
        {
            MudDialog.Close(DialogResult.Ok(UserViewModel));
        }
    }
}