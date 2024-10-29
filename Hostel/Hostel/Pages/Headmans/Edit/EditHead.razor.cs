using Microsoft.AspNetCore.Components;
using MudBlazor;
using Suo.Admin.Data.Models.Request;
using System.Text;

namespace Suo.Admin.Pages.Headmans
{
    public class EditHeadView : ComponentBase
    {
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
        [Parameter]
        public UserAddRequest UserAdd { get; set; } = new UserAddRequest();
        [Parameter]
        public string Title { get; set; }



        public void Cancel()
        {
            MudDialog.Cancel();
        }
        public void Save()
        {
            MudDialog.Close(DialogResult.Ok(UserAdd));
        }

        public void GeneratePassword()
        {
            var chars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ!*#?";

            StringBuilder sb = new StringBuilder();
            Random rnd = new Random();

            for (int i = 0; i < 12; i++)
            {
                int index = rnd.Next(chars.Length);
                sb.Append(chars[index]);
            }
            UserAdd.Password = sb.ToString();
        }
    }
}
