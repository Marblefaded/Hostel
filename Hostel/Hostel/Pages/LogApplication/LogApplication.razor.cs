using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Suo.Admin.Data.Service;
using Suo.Admin.Data.ViewModel;
using Suo.Admin.Pages.User.Edit;
using Suo.Admin.Shared;

namespace Suo.Admin.Pages.LogApplication
{
    public class LogApplicationView : ComponentBase
    {
        [Inject] protected IDialogService DialogService { get; set; }
        [Inject] private LogApplicationService LogService { get; set; }
        [Inject] protected ISnackbar Snackbar { get; set; }
        [Inject] protected IJSRuntime jsruntime { get; set; }


        protected List<LogApplicationViewModel> Model { get; set; }
        protected List<LogApplicationViewModel> ModelForClearing = new List<LogApplicationViewModel>();
        public bool flag;
        protected LogApplicationStackTraceView StackTraceModel = new LogApplicationStackTraceView();
        public LogApplicationViewModel CurrentItem;
        public DateTime filterValue { get; set; }
        public string filterError = "";

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                FilterValue = DateTime.Now;
                Model = await LogService.GetAll();
                //ModelForClearing = await LogService.GetAll();
                //Model.Reverse();
                await InvokeAsync(StateHasChanged);
            }
        }

        public DateTime FilterValue
        {
            get => filterValue;

            set
            {
                filterValue = value;
                Filter();
            }
        }
        public string FilterError
        {
            get => filterError;

            set
            {
                filterError = value;
                FiltersError();
            }
        }
        public async Task DeleteItemForeverAsync(LogApplicationViewModel item)
        {
            try
            {
                var dialog = DialogService.Show<DeleteComponent>("Вы точно хотите удалить данную запись?");
                var result = await dialog.Result;
                if (!result.Canceled)
                {
                    LogService.Delete(item);
                    Model.Remove(item);
                }

                StateHasChanged();
            }
            catch (Exception ex)
            {

            }
        }
        public async Task DeleteItemAsync(LogApplicationViewModel item)
        {
            try
            {
                /*var dialog = DialogService.Show<DeleteComponent>("Вы точно хотите удалить данную запись?");
                var result = await dialog.Result;
                if (!result.Canceled)
                {
                    LogService.Delete(item);
                    Model.Remove(item);
                }*/
                item.IsDeleted = true;
                await InvokeAsync(StateHasChanged);
            }
            catch (Exception ex)
            {

            }
        }
        public async Task RestoreItemAsync(LogApplicationViewModel item)
        {
            try
            {
                item.IsDeleted = false;
                StateHasChanged();
            }
            catch (Exception ex)
            {

            }
        }
        public static bool Enable(LogApplicationViewModel viewModel)
        {
            if(viewModel.IsEnableDelete == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task DeleteSelectedItemAsync()
        {
            List<LogApplicationViewModel> results = Model.FindAll(Enable);
            if (results.Count != 0)
            {
                var dialog = DialogService.Show<DeleteComponent>("Вы точно хотите удалить выбранные записи?");
                var result = await dialog.Result;
                if (!result.Canceled)
                {
                    foreach (var items in Model)
                    {
                        if (items.IsEnableDelete == true)
                        {
                            ModelForClearing.Add(items);
                        }
                    }
                    foreach (var model in ModelForClearing)
                    {
                        Model.Remove(model);
                        LogService.Delete(model);
                        Snackbar.Add("Элементы успешно удалены", Severity.Success);
                    }
                }

            }
            else
            {
                Snackbar.Add("Нет выделенных элементов", Severity.Error);
            }
        }

        public async Task DeleteAllItemsAsync()
        {
            try
            {
                LogService.DeleteAllLogs();
                Model.Clear();
                Snackbar.Add("Таблица очищена", Severity.Success);
                StateHasChanged();
            }
            catch (Exception ex)
            {

            }
        }
        public async Task ExportToExcel(LogApplicationViewModel item)
        {
            CurrentItem = item;

            IWorkbook workbook = new XSSFWorkbook();

            var dataFormat = workbook.CreateDataFormat();
            var dataStyle = workbook.CreateCellStyle();
            dataStyle.DataFormat = dataFormat.GetFormat("dd.MM.yyyy");

            ISheet worksheet = workbook.CreateSheet("Sheet1");

            int rowNumber = 0;
            IRow row = worksheet.CreateRow(rowNumber++);

            //Заголовок таблицы
            ICell cell = row.CreateCell(0);
            cell.SetCellValue("Id");

            cell = row.CreateCell(1);
            cell.SetCellValue("Message");

            cell = row.CreateCell(2);
            cell.SetCellValue("StackTrace");

            cell = row.CreateCell(3);
            cell.SetCellValue("Date");

            cell = row.CreateCell(4);
            cell.SetCellValue("User");

            //Тело таблицы

            foreach (var model in Model)
            {
                row = worksheet.CreateRow(rowNumber++);
                //Id
                cell = row.CreateCell(0);
                cell.SetCellValue(model.LogApplicationId);
                //Сообщение об ошибке
                cell = row.CreateCell(1);
                cell.SetCellValue(model.Message);
                //StackTrace
                cell = row.CreateCell(2);
                cell.SetCellValue(model.ErrorContext);
                //Дата
                cell = row.CreateCell(3);
                cell.SetCellValue(model.Date);
                //Пользователь
                cell = row.CreateCell(4);
                cell.SetCellValue(model.UserName);
            }

            MemoryStream ms = new MemoryStream();
            workbook.Write(ms, false);
            byte[] bytes = ms.ToArray();
            ms.Close();

            await SaveAsFileAsync(jsruntime, "LogApplicationSYO.xlsx", bytes, "application/vnd.ms-excel");

        }

        public async Task SaveAsFileAsync(IJSRuntime js, string filename, byte[] data, string type = "application/octet-stream")
        {
            await js.InvokeAsync<object>("saveAsFile", filename, Convert.ToBase64String(data));
        }
        public void Change(LogApplicationViewModel item)
        {
            try
            {
                if (item.IsEnableDelete == true)
                {
                    item.IsEnableDelete = false;
                    /* ModelForClearing.Remove(item);*/
                }
                else
                {
                    item.IsEnableDelete = true;
                    /*ModelForClearing.Add(item);*/
                }
                LogService.Update(item);
            }
            catch (Exception ex)
            {

            }

        }
        public void AllChange()
        {
            foreach(var change in Model)
            {
                if (change.IsEnableDelete == true)
                {
                    change.IsEnableDelete = false;
                    /* ModelForClearing.Remove(item);*/
                }
                else
                {
                    change.IsEnableDelete = true;
                    /*ModelForClearing.Add(item);*/
                }
            }

        }

        public async Task StackTraceDialog(LogApplicationViewModel item)
        {
            try
            {
                var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Medium };
                var parameters = new DialogParameters<LogApplicationStackTrace> { { x => x.LogViewModel, item } };
                parameters.Add(x => x.Title, "Трассировка стека");
                var dialog = DialogService.Show<LogApplicationStackTrace>("", parameters, options);
                var result = await dialog.Result;
                if (!result.Canceled)
                {
                    
                }

            }
            catch (Exception ex)
            {
                /*LogService.Create(LogModel, ex.Message, ex.StackTrace, DateTime.Now);*/
            }
        }
        protected void Filter()
        {
            Model = LogService.Filtering(filterValue);
            StateHasChanged();
        }
        protected void FiltersError()
        {
            Model = LogService.FilteringError(filterError);
            StateHasChanged();
        }
    }
}
