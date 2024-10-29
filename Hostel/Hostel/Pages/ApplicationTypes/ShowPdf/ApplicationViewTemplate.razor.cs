using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using NPOIPdfEngine;
using NPOIPdfEngine.Models;
using Suo.Admin.Data.EditModel;
using Suo.Admin.Data.Service;
using Suo.Admin.Data.ViewModel;

namespace Suo.Admin.Pages.ApplicationTypes.ShowPdf
{
    public class ApplicationViewTemplateScreen : ComponentBase
    {
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
        [Parameter]
        public ClaimTemplateViewModel mTemplateItem { get; set; }
        public EditClaimTemplateItemViewModel ViewModel { get; set; }

        [Inject] protected IJSRuntime jsRunTime { get; set; }
        [Inject] private LogApplicationService LogService { get; set; }
        [Inject] protected Engine npoiEngine { get; set; }
        public LogApplicationViewModel LogModel = new LogApplicationViewModel();
        public string Data { get; set; }
        public byte[] bytes1;

        protected override void OnParametersSet()
        {
            ReportModel rModel = new ReportModel();
            EditClaimTemplateItemViewModel ViewModel = new EditClaimTemplateItemViewModel();
            ViewModel.report = System.Text.Json.JsonSerializer.Deserialize<ReportModel>(mTemplateItem.TemplateModelJson);

            rModel.Facultet = ViewModel.report.Facultet;
            rModel.Kurs = ViewModel.report.Kurs;
            rModel.Group = ViewModel.report.Group;
            rModel.Firstname = ViewModel.report.Firstname;
            rModel.Middlename = ViewModel.report.Middlename;
            rModel.Lastname = ViewModel.report.Lastname;
            rModel.Room = ViewModel.report.Room;
            rModel.Phonenumber = ViewModel.report.Phonenumber;
            rModel.Reason = ViewModel.report.Reason;
            rModel.Dateofapplication = ViewModel.report.Dateofapplication;

            rModel.Text = ViewModel.report.Text;
            rModel.NameOfManager = ViewModel.report.NameOfManager;
            rModel.RankOfManagement = ViewModel.report.RankOfManagement;
            rModel.CreateDate = ViewModel.report.CreateDate;
            rModel.TimeToEnd = ViewModel.report.TimeToEnd;
            rModel.TimeToGo = ViewModel.report.TimeToGo;
            rModel.TimeToStart = ViewModel.report.TimeToStart;
            bytes1 = npoiEngine.CreateReportPdf(rModel);
            convertImageToDisplay(bytes1);
            
            //StateHasChanged();
        }
        public void convertImageToDisplay(byte[] bytes)
        {
            Data = Convert.ToBase64String(bytes);
        }
        public async Task StateChanged(byte[] dataTemplate)
        {
            Data = null;
            StateHasChanged();
            await Task.Delay(1);
            Data = Convert.ToBase64String(dataTemplate);
            StateHasChanged();
        }

        public async Task DownloadPdf(ClaimTemplateViewModel item)
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                byte[] bytes = item.DataTemplate;
                ms.Close();

                await SaveAsFileAsync(jsRunTime, $"{item.Title}.pdf", bytes);
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
        public void Cancel()
        {
            MudDialog.Cancel();
        }
    }
}
