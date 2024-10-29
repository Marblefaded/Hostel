using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using MudBlazor;
using NPOIPdfEngine;
using NPOIPdfEngine.Models;
using Suo.Admin.Data.EditModel;
using Suo.Admin.Data.Service;
using Suo.Admin.Data.ViewModel;
using Suo.Admin.Pages.ApplicationTypes.Create;
using Suo.Admin.Pages.ApplicationTypes.Edit;
using Suo.Admin.Pages.ApplicationTypes.ShowPdf;
using Suo.Admin.Shared;

namespace Suo.Admin.Pages.ApplicationTypes
{
    public class ApplicationTypesView : ComponentBase
    {
        [Inject] protected IDialogService DialogService { get; set; }
        [Inject] protected IJSRuntime jsRunTime { get; set; }
        [Inject] protected ISnackbar Snackbar { get; set; }
        [Inject] protected ClaimTemplateService Service { get; set; }
        [Inject] protected ClaimService ClaimService { get; set; }
        [Inject] protected Engine npoiEngine { get; set; }
        [Inject] private LogApplicationService LogService { get; set; }
        public LogApplicationViewModel LogModel = new LogApplicationViewModel();
        public List<ClaimTemplateViewModel> Model { get; set; }
        public List<ClaimViewModel> ClaimModel { get; set; } = new();
        public ClaimViewModel mClaim { get; set; } = new();

        public ClaimTemplateViewModel mCurrentItem;
        public ClaimTemplateViewModel mItem;
        public EditClaimTemplateItemViewModel mEditViewModel = new EditClaimTemplateItemViewModel();
        public EditClaimTemplateItemViewModel viewModel { get; set; }
        public ClaimTemplateViewModel claimview = new ClaimTemplateViewModel();
        ApplicationViewTemplate editView = new ApplicationViewTemplate();

        public string Data { get; set; }
        public byte[] bytes1 { get; set; }
        public bool isRemove;
        public int[] claims;
        public ClaimViewModel returnModel = new ClaimViewModel();
        //public bool isOpenTemplate;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            try 
            {
                if (firstRender)
                {
                    ClaimModel = await ClaimService.GetAll();
                    Model = await Service.GetAll();
                    await InvokeAsync(StateHasChanged);
                }
            }
            catch (Exception ex)
            {
                LogService.Create(LogModel, ex.Message, ex.StackTrace, DateTime.Now);
            }
        }

        public async Task AddItemDialog()
        {
            mItem = new ClaimTemplateViewModel();
            mEditViewModel.Item = mItem;
            mEditViewModel.report = new ReportModel();

            var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Medium };
            var parameters = new DialogParameters<ApplicationEdit> { { x => x.ViewModel, mEditViewModel } };
            var dialog = DialogService.Show<ApplicationEdit>("Новый тип заявления", parameters, options);
            var result = await dialog.Result;
            if (!result.Canceled)
            {
                ClaimTemplateViewModel returnModel = new ClaimTemplateViewModel();
                returnModel = (ClaimTemplateViewModel)result.Data;
                SaveTemplate(returnModel);               
                StateHasChanged();
            }
        }

        public async Task EditItemDialog(ClaimTemplateViewModel item)
        {
            mItem = item;
            mCurrentItem = item.Clone() as ClaimTemplateViewModel;
            mEditViewModel.Item = mItem;
            mEditViewModel.report = new ReportModel();
            mEditViewModel.report = System.Text.Json.JsonSerializer.Deserialize<ReportModel>(mCurrentItem.TemplateModelJson);

            var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Medium };
            var parameters = new DialogParameters<TemplateEdit> { { x => x.ViewModel, mEditViewModel } };
            var dialog = DialogService.Show<TemplateEdit>("Новый тип заявления", parameters, options);
            var result = await dialog.Result;
            if (!result.Canceled)
            {
                ClaimTemplateViewModel returnModel = new ClaimTemplateViewModel();
                returnModel = (ClaimTemplateViewModel)result.Data;
                SaveTemplate(returnModel);
                StateHasChanged();
            }
            else
            {
                ClaimModel = await ClaimService.GetAll();
                Model = await Service.GetAll();
                await InvokeAsync(StateHasChanged);
            }
        }

        public async Task OpenFileDialogAsync(ClaimTemplateViewModel item)
        {
            try
            {
                
                var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Medium };
                var parameters = new DialogParameters<ApplicationViewTemplate> { { x => x.mTemplateItem, item } };
                var dialog = DialogService.Show<ApplicationViewTemplate>($"Просмотр {item.Title}", parameters, options);
                var result = await dialog.Result;
            }
            catch (Exception ex)
            {
                LogService.Create(LogModel, ex.Message, ex.StackTrace, DateTime.Now);
            }
        }

        public async Task DeleteItemAsync(ClaimTemplateViewModel item)
        {
            try
            {
                var dialog = DialogService.Show<DeleteComponent>("Вы точно хотите удалить данный вид заявления?");
                var result = await dialog.Result;
                if (!result.Canceled)
                {
                    Service.Delete(item);
                    Model.Remove(item);
                }                
                StateHasChanged();
            }
            catch (Exception ex)
            {
                LogService.Create(LogModel, ex.Message, ex.StackTrace, DateTime.Now);
            }
        }
      
        public void SaveTemplate(ClaimTemplateViewModel item)
        {
            try
            {
                if (item.ClaimTemplateId > 0)
                {
                    var newItem = Service.Update(item);
                    var index = Model.FindIndex(x => x.ClaimTemplateId == newItem.ClaimTemplateId);
                    Model[index] = newItem;
                }
                else
                {
                    var newItem = Service.Create(item);
                    Model.Add(newItem);
                }              
                StateHasChanged();

            }
            catch (Exception ex)
            {
                LogService.Create(LogModel, ex.Message, ex.StackTrace, DateTime.Now);
                if (ex is DbUpdateConcurrencyException)
                {
                    mEditViewModel.IsConcurency = true;
                    mEditViewModel.ConcurencyErrorText = "Данные не актуальны! Обновите страницу";
                }
                return;

            }

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
    }
}
