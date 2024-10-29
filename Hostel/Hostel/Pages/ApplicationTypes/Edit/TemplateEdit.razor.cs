using Microsoft.AspNetCore.Components;
using MudBlazor;
using NPOIPdfEngine;
using NPOIPdfEngine.Models;
using Suo.Admin.Data.EditModel;
using Suo.Admin.Data.ViewModel;

namespace Suo.Admin.Pages.ApplicationTypes.Edit
{
    public class TemplateEditView : ComponentBase
    {
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
        [Inject] protected Engine npoiEngine { get; set; }
        [Parameter]
        public EditClaimTemplateItemViewModel ViewModel { get; set; }
        public ClaimTemplateViewModel claimview = new ClaimTemplateViewModel();
        public string Data { get; set; }
        public byte[] bytes1;
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
            //ViewModel.report.Reason = "[reason]";
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
            claimview.ClaimTemplateId = ViewModel.Item.ClaimTemplateId; 
            claimview.Title = ViewModel.Item.Title;
            claimview.ChangeLog = "create template";
            claimview.ClaimTypeId = 1;
            claimview.ClaimJson = $@"{ViewModel.Item.Title}.pdf";

            ViewModel.report.CreateDate = DateTime.Now.Date;

            claimview.TemplateModelJson = System.Text.Json.JsonSerializer.Serialize(ViewModel.report);
            bytes1 = npoiEngine.CreateReportPdf(ViewModel.report);
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

            //rModel.Facultet= ViewModel.report.Facultet;
            //rModel.Kurs = ViewModel.report.Kurs;
            //rModel.Group = ViewModel.report.Group;
            //rModel.Firstname = ViewModel.report.Firstname;
            //rModel.Middlename = ViewModel.report.Middlename;
            //rModel.Lastname = ViewModel.report.Lastname;
            //rModel.Room = ViewModel.report.Room;
            //rModel.Phonenumber = ViewModel.report.Phonenumber;
            //rModel.Reason = ViewModel.report.Reason;
            //rModel.Dateofapplication = ViewModel.report.Dateofapplication;

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
            rModel.TimeToEnd = ViewModel.report.TimeToEnd;
            rModel.TimeToGo = ViewModel.report.TimeToGo;
            rModel.TimeToStart = ViewModel.report.TimeToStart;

            bytes1 = npoiEngine.CreateReportPdf(rModel);
            convertImageToDisplay(bytes1);
            StateHasChanged();
            return;
        }

        //public async Task Find(ChangeEventArgs e)
        //{
        //    if (e.Value == null)
        //    {
        //        return;
        //    }

        //    ReportModel rModel = new ReportModel();
        //    rModel.Text = e.Value.ToString();
        //    var x = rModel.CreateDate.ToString();
        //    x = e.Value.ToString();

        //    bytes1 = npoiEngine.CreateReportPdf(rModel);

        //    convertImageToDisplay(bytes1);

        //    StateHasChanged();

        //    return;
        //}
        public void Cancel()
        {
            MudDialog.Cancel();
        }
    }
}
