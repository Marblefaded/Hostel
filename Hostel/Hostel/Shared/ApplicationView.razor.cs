using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Suo.Admin.Data.Service;
using Suo.Admin.Data.ViewModel;

namespace Suo.Admin.Shared
{
    public class ApplicationViewScreen : ComponentBase
    {
        [Parameter]
        public ClaimViewModel mCurrentItem { get; set; }
        protected List<UserViewModel> UserModel { get; set; } = new();
        protected List<ClaimTemplateViewModel> TemplateModel { get; set; } = new();
        [Inject] protected IJSRuntime jsRunTime { get; set; }
        [Inject] private LogApplicationService LogService { get; set; }
        [Inject] protected UserService userService { get; set; }
        [Inject] protected ClaimTemplateService templateService { get; set; }
        public LogApplicationViewModel LogModel = new LogApplicationViewModel();
        public string Data { get; set; }

        

        protected override void OnParametersSet()
        {
            Data = Convert.ToBase64String(mCurrentItem.DataClaim);//ошибка
            UserModel = userService.Get();
            TemplateModel = templateService.Get();
            StateHasChanged();
        }

        public async Task StateChanged(byte[] dataClaim)
        {
            Data = null;
            StateHasChanged();
            await Task.Delay(1);
            Data = Convert.ToBase64String(dataClaim);
            StateHasChanged();
        }

        public async Task DownloadPdf(ClaimViewModel item)
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                byte[] bytes = item.DataClaim;
                ms.Close();

                await SaveAsFileAsync(jsRunTime, $"{TemplateModel.FirstOrDefault(x => x.ClaimTemplateId == item.ClaimTemplateId).Title}\n{UserModel.FirstOrDefault(x => x.UserId == item.UserId).Surname}.pdf", bytes);
            }
            catch (Exception ex)
            {
                LogService.Create(LogModel, ex.Message, ex.StackTrace, DateTime.Now);
            }
        }

        public async Task SaveAsFileAsync(IJSRuntime js, string filename, byte[] data)
        {
            try
            {
                await js.InvokeAsync<object>("saveAsFile", filename, Convert.ToBase64String(data));
            }
            catch (Exception ex)
            {
                LogService.Create(LogModel, ex.Message, ex.StackTrace, DateTime.Now);
            }
        }

        /*public void convertPdfToDisplay(byte[] bytes)
        {
            Data = Convert.ToBase64String(bytes);
        }
        public void ShowPdf()
        {
            var data = mCurrentItem.DataClaim;
            if(data != null)
            {
                convertPdfToDisplay(data);
            }
        }*/
    }
}
