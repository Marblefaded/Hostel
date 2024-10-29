using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using NPOIPdfEngine;
using NPOIPdfEngine.Models;
using Spire.Pdf.Exporting.XPS.Schema;
using Suo.Admin.Data.EditModel;
using Suo.Admin.Data.Service;
using Suo.Admin.Data.ViewModel;

namespace Suo.Admin.Pages.ApplicationTypes.Create
{
    public class ApplicationEditView : ComponentBase
    {
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
        [Inject] protected Engine npoiEngine { get; set; }
        [Inject] protected ClaimTemplateService Service { get; set; }
        [Inject] private LogApplicationService LogService { get; set; }
        [Parameter]
        public EditClaimTemplateItemViewModel ViewModel { get; set; }

        public LogApplicationViewModel LogModel = new LogApplicationViewModel();

        public ClaimTemplateViewModel claimview = new ClaimTemplateViewModel();
        public ClaimTemplateViewModel mItem;
        public string Data { get; set; }
        public byte[] bytes1;
        public string headers;

        protected override async Task OnInitializedAsync()
        {
            ViewModel.report.Facultet = "[facultet]";
            ViewModel.report.Kurs = "[kurs]";
            ViewModel.report.Group = "[group]";
            ViewModel.report.Firstname = "[firstname]";
            ViewModel.report.Middlename = "[middlename]";
            ViewModel.report.Lastname = "[lastname]";
            ViewModel.report.Room = "[room]";
            ViewModel.report.Phonenumber = "[phonenumber]";
            ViewModel.report.Dateofapplication = "[dateofapplication]";
            ViewModel.report.NameOfManager = "[nameofmanager]";
            ViewModel.report.RankOfManagement = "[rankofmanagement]";
        }
        public void convertImageToDisplay(byte[] bytes)
        {
            Data = Convert.ToBase64String(bytes);
        }
        public async void SaveTask()
        {
            claimview.Title = ViewModel.Item.Title;
            claimview.ChangeLog = "create template";
            claimview.ClaimTypeId = 1;
            claimview.ClaimJson = $@"{ViewModel.Item.Title}.pdf";
            //ViewModel.report.Student = "[firstname]";
            ViewModel.report.CreateDate = DateTime.Now.Date;

            if (ViewModel.report.Text?.IndexOf("[timetogo]") > 0)
            {
                ViewModel.report.TimeToGo = "[timetogo]";
            }

            if (ViewModel.report.Text?.IndexOf("[nameofmanager]") > 0)
            {
                ViewModel.report.NameOfManager = "[nameofmanager]";
            }

            if (ViewModel.report.Text?.IndexOf("[rankofmanagement]") > 0)
            {
                ViewModel.report.RankOfManagement = "[rankofmanagement]";
            }


            if (ViewModel.report.Text?.IndexOf("[timetostart]") > 0)
            {
                ViewModel.report.TimeToStart = "[timetostart]";
                //rModel.TimeToGo = "[timetostart]";
            }

            if (ViewModel.report.Text?.IndexOf("[timetoend]") > 0)
            {
                ViewModel.report.TimeToEnd = "[timetoend]";
                //rModel.TimeToGo = "[timetoend]";
            }
            if (ViewModel.report.Text?.IndexOf("[reason]") > 0)
            {
                ViewModel.report.Reason = "[reason]";
                //rModel.Reason = "[reason]";
            }
            claimview.TemplateModelJson = System.Text.Json.JsonSerializer.Serialize(ViewModel.report);
            claimview.DataTemplate = bytes1;
            MudDialog.Close(DialogResult.Ok(claimview));
            await InvokeAsync(StateHasChanged);
        }

        public async Task Find()
        {

            ReportModel rModel = new ReportModel();
            rModel.Text = ViewModel.report.Text;
            rModel.NameOfManager = ViewModel.report.NameOfManager;
            rModel.RankOfManagement = ViewModel.report.RankOfManagement;
            rModel.CreateDate = ViewModel.report.CreateDate;
            rModel.Facultet = ViewModel.report.Facultet;
            rModel.Kurs = ViewModel.report.Kurs;
            rModel.Group = ViewModel.report.Group;
            rModel.Firstname = ViewModel.report.Firstname;
            rModel.Middlename = ViewModel.report.Middlename;
            rModel.Lastname = ViewModel.report.Lastname;
            rModel.Room = ViewModel.report.Room;
            rModel.Phonenumber = ViewModel.report.Phonenumber;
            //rModel.Reason = ViewModel.report.Reason;
            rModel.TimeToGo = ViewModel.report.TimeToGo;
            rModel.Dateofapplication = ViewModel.report.Dateofapplication;



            if (ViewModel.report.Text?.IndexOf("[reason]") > 0)
            {
                ViewModel.report.Reason = "[reason]";
                rModel.Reason = "[reason]";
            }

            if (ViewModel.report.Text?.IndexOf("[rankofmanagement]") > 0)
            {
                ViewModel.report.RankOfManagement = "[rankofmanagement]";
                rModel.RankOfManagement = "[rankofmanagement]";
            }

            if (ViewModel.report.Text?.IndexOf("[nameofmanager]") > 0)
            {
                ViewModel.report.NameOfManager = "[nameofmanager]";
                rModel.NameOfManager = "[nameofmanager]";
            }

            if (ViewModel.report.Text?.IndexOf("[timetogo]") > 0)
            {
                ViewModel.report.TimeToGo = "[timetogo]";
                rModel.TimeToGo = "[timetogo]";
            }

            if (ViewModel.report.Text?.IndexOf("[timetostart]") > 0)
            {
                ViewModel.report.TimeToStart = "[timetostart]";
                rModel.TimeToStart = "[timetostart]";
            }

            if (ViewModel.report.Text?.IndexOf("[timetoend]") > 0)
            {
                ViewModel.report.TimeToEnd = "[timetoend]";
                rModel.TimeToEnd = "[timetoend]";
            }

            bytes1 = npoiEngine.CreateReportPdf(rModel);

            convertImageToDisplay(bytes1);

            StateHasChanged();

            return;
        }
        public async Task Header()
        {
            ReportModel rModel = new ReportModel();
            //rModel.NameOfManager = ViewModel.report.NameOfManager;
            //rModel.RankOfManagement = ViewModel.report.RankOfManagement;
            StateHasChanged();

            return;
        }
        public void Change(ClaimTemplateViewModel item)
        {
            try
            {
                if (item.ClaimTemplateId == 1)
                {
                    item.ClaimTemplateId = 2;
                }
                else item.ClaimTemplateId = 1;
                Service.Update(item);
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
