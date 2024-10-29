using HostelDB.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using NPOI.HSSF.UserModel;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using Spire.Xls;
using Suo.Admin.Data.Service;
using Suo.Admin.Data.ViewModel;


namespace Suo.Admin.Pages.ExportData
{
    public class ExportDataView : ComponentBase

    {
        public ExportDataChoiceViewModel _choiceModel;

        public List<UserViewModel> UserViewModels { get; set; }
        public List<RoomViewModel> RoomViewModels { get; set; }
        public List<UserRoomViewModel> UserRoomViewModels { get; set; }
        public List<ExportDataViewModel> ExportDataViewModels { get; set; }
        public LogApplicationViewModel LogModel = new LogApplicationViewModel();

        [Inject] protected IJSRuntime js { get; set; }
        [Inject] protected UserRoomService UserRoomService { get; set; }
        [Inject] protected UserService UserService { get; set; }
        [Inject] protected RoomService RoomService { get; set; }
        [Inject] private LogApplicationService LogApplicationService { get; set; }
        protected override async Task OnInitializedAsync()
        {
            UserViewModels = UserService.Get();
            RoomViewModels = RoomService.Get();
            UserRoomViewModels = UserRoomService.Get();
            _choiceModel = new ExportDataChoiceViewModel();
            GenerateDataForExport();
        }

        public void GenerateDataForExport()
        {
            try
            {
                ExportDataViewModels = new();
                foreach (var item in UserViewModels)
                {
                    RoomViewModel room = new();
                    //На случай жильцов без комнат.
                    try
                    {
                        room = RoomViewModels.FirstOrDefault(x => x.RoomId == UserRoomViewModels.FirstOrDefault(x => x.UserId == item.UserId).RoomId);
                    }
                    catch (NullReferenceException)
                    {
                    }
                    //Возможен рефакторинг с партиал вью.
                    var exportModel = new ExportDataViewModel();
                    if (_choiceModel.Room) { exportModel.Room = room.NumberRoom; }
                    if (_choiceModel.UserName) { exportModel.UserName = item.Name; }
                    if (_choiceModel.UserLastName) { exportModel.UserLastName = item.Surname; }
                    if (_choiceModel.UserPatronymic) { exportModel.UserPatronymic = item.Secondname; }
                    if (_choiceModel.UserPhoneNumber) { exportModel.UserPhoneNumber = item.PhoneNumber; }
                    if (_choiceModel.UserCourse) { exportModel.UserCourse = item.UserCourse; }
                    if (_choiceModel.UserDeportament) { exportModel.UserDeportament = item.UserDeportament; }
                    if (_choiceModel.UserGroup) { exportModel.UserGroup = item.UserGroup; }
                    exportModel.Floor = room.Floor.ToString();
                    ExportDataViewModels.Add(exportModel);
                }
            }
            catch (Exception ex)
            {
                LogApplicationService.Create(LogModel, ex.Message, ex.StackTrace, DateTime.Now);
            }
        }
        public async void ExportData(Dictionary<int, List<ExportDataViewModel>> sortedByFloorsDataList)
        {
            try
            {
                IWorkbook workbook = new HSSFWorkbook();

                foreach (var list in sortedByFloorsDataList)
                {
                    var sheet = workbook.CreateSheet($"{list.Key} этаж");

                    var ListOfNeededData = new List<string>();
                    if (_choiceModel.Room) { ListOfNeededData.Add("Комната"); }
                    if (_choiceModel.UserName) { ListOfNeededData.Add("Имя"); }
                    if (_choiceModel.UserLastName) { ListOfNeededData.Add("Фамилия"); }
                    if (_choiceModel.UserPatronymic) { ListOfNeededData.Add("Отчество"); }
                    if (_choiceModel.UserPhoneNumber) { ListOfNeededData.Add("Номер телефона"); }
                    if (_choiceModel.UserGroup) { ListOfNeededData.Add("Группа"); }
                    if (_choiceModel.UserCourse) { ListOfNeededData.Add("Курс"); }
                    if (_choiceModel.UserDeportament) { ListOfNeededData.Add("Факультет"); }

                    var headerRow = sheet.CreateRow(0);
                    for (var i = 0; i < ListOfNeededData.Count(); i++)
                    {
                        headerRow.CreateCell(i).SetCellValue(ListOfNeededData[i]);
                    }
                    var iteratorForRow = 1;
                    var nextRow = sheet.CreateRow(iteratorForRow);

                    foreach (var item in list.Value)
                    {
                        var iterator = 0;
                        if (_choiceModel.Room) { nextRow.CreateCell(iterator).SetCellValue(item.Room); iterator++; }
                        if (_choiceModel.UserName) { nextRow.CreateCell(iterator).SetCellValue(item.UserName); iterator++; }
                        if (_choiceModel.UserLastName) { nextRow.CreateCell(iterator).SetCellValue(item.UserLastName); iterator++; }
                        if (_choiceModel.UserPatronymic) { nextRow.CreateCell(iterator).SetCellValue(item.UserPatronymic); iterator++; }
                        if (_choiceModel.UserPhoneNumber) { nextRow.CreateCell(iterator).SetCellValue(item.UserPhoneNumber); iterator++; }
                        if (_choiceModel.UserGroup) { nextRow.CreateCell(iterator).SetCellValue(item.UserGroup); iterator++; }
                        if (_choiceModel.UserCourse) { nextRow.CreateCell(iterator).SetCellValue(item.UserCourse); iterator++; }
                        if (_choiceModel.UserDeportament) { nextRow.CreateCell(iterator).SetCellValue(item.UserDeportament); iterator++; }
                        iteratorForRow++;
                        nextRow = sheet.CreateRow(iteratorForRow);
                    }
                }
                MemoryStream output = new MemoryStream();
                workbook.Write(output);
                byte[] bytes = output.ToArray();
                output.Close();
                await SaveAsFileAsync(js, $"{DateTime.Now.ToShortDateString()}.xls", bytes);
            }
            catch (Exception ex)
            {
                LogApplicationService.Create(LogModel, ex.Message, ex.StackTrace, DateTime.Now);
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
                LogApplicationService.Create(LogModel, ex.Message, ex.StackTrace, DateTime.Now);
            }
        }
        public void SortAndDownload()
        {
            try
            {
                Dictionary<int, List<ExportDataViewModel>> sortedByFloorsDataList = new()
            {
                { 3, ExportDataViewModels.Where(x => x.Floor == "3").ToList() },
                { 4, ExportDataViewModels.Where(x=>x.Floor=="4").ToList()},
                { 5, ExportDataViewModels.Where(x=>x.Floor=="5").ToList() }
            };
                ExportData(sortedByFloorsDataList);
            }
            catch (Exception ex)
            {
                LogApplicationService.Create(LogModel, ex.Message, ex.StackTrace, DateTime.Now);
            }
        }
    }
}
