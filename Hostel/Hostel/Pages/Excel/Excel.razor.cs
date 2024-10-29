using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using MudBlazor;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using Suo.Admin.Data.Service;
using Suo.Admin.Data.ViewModel;


namespace Suo.Admin.Pages.Excel
{
    public class ExcelView : ComponentBase
    {
        [Inject]
        protected IJSRuntime Js { get; set; }
        [Inject]
        protected IWebHostEnvironment HostingEnv { get; set; }
        [Inject]
        protected UserService UserService { get; set; }
        [Inject]
        protected RoomService RoomService { get; set; }
        [Inject]
        protected UserRoomService UserRoomService { get; set; }
        [Inject] private LogApplicationService LogService { get; set; }
        [Inject] private TelegramUserService TeleService { get; set; }

        [Inject] protected ISnackbar Snackbar { get; set; }


        public LogApplicationViewModel LogModel = new LogApplicationViewModel();
        public ConfirmationView ConfirmModel = new ConfirmationView();
        public ErrorMessageView ErrorModel = new ErrorMessageView();

        protected List<RoomViewModel> RoomModel = new List<RoomViewModel>();
        protected List<UserViewModel> UserModel = new List<UserViewModel>();
        protected List<RoomViewModel> ListNewRooms = new List<RoomViewModel>();
        protected List<UserViewModel> ListNewUser = new List<UserViewModel>();
        protected List<UserRoomViewModel> ListNewUserRooms = new List<UserRoomViewModel>();


        public bool flag;
        public bool flagForProcessing;
        private List<IBrowserFile> loadedFiles = new();
        private long maxFileSize = 1024 * 1024 * 15;
        private int maxAllowedFiles = 1;
        private bool isLoading;
        private List<string> newFiles = new List<string>();
        private string fileNameExcel;


        public async Task SaveFiles(InputFileChangeEventArgs e)
        {
            try
            {
                isLoading = true;
                loadedFiles.Clear();
                flag = false;
                flagForProcessing = false;
                var pathFolder = Path.Combine(HostingEnv.WebRootPath, "Excel");

                if (Directory.Exists(pathFolder))
                {
                    //    Console.WriteLine("\nПапка 'Excel' уже существует или была успешно создана.\n");
                }
                else
                {
                    Directory.CreateDirectory(pathFolder);
                }

                foreach (var file in e.GetMultipleFiles(maxAllowedFiles))
                {
                    loadedFiles.Add(file);
                    var trustedFileNameForFileStorage = Guid.NewGuid().ToString();
                    var fileExtension = Path.GetExtension(file.Name);
                    var fileName = trustedFileNameForFileStorage + fileExtension;
                    var path = Path.Combine(pathFolder, fileName);

                    fileNameExcel = path;

                    using (var fs = new FileStream(path, FileMode.Create))
                    {
                        await file.OpenReadStream(maxFileSize).CopyToAsync(fs);
                    }
                    newFiles.Add(path);
                    //Console.WriteLine("File saved successfully: " + path);
                }
                isLoading = false;

                if (fileNameExcel.Substring(fileNameExcel.Length - 4) != "xlsx")
                {
                    File.Delete(fileNameExcel);
                    ErrorModel.ErrorMessage = "На вход подан неверный файл";
                    ErrorModel.DialogIsOpen = true;
                    return;
                }

                ListNewUser.Clear();
                ListNewRooms.Clear();
                ListNewUserRooms.Clear();

                RoomModel = RoomService.Get();
                UserModel = UserService.Get();
                string RoomNumber;

                using (FileStream file = new FileStream(fileNameExcel, FileMode.Open, FileAccess.Read))
                {
                    IWorkbook workbook = new XSSFWorkbook(file);

                    for (var sheetiter = 0; sheetiter < workbook.NumberOfSheets; sheetiter++)
                    {
                        ISheet sheet = workbook.GetSheetAt(sheetiter);
                        IRow firstRow = sheet.GetRow(sheet.FirstRowNum);

                        var firstCellCheck = firstRow.GetCell(CellReference.ConvertColStringToIndex("A"));
                        var secondCellCheck = firstRow.GetCell(CellReference.ConvertColStringToIndex("C"));
                        var thirdCellCheck = firstRow.GetCell(CellReference.ConvertColStringToIndex("H"));
                        string firstCell = null;
                        string secondCell = null;
                        string thirdCell = null;
                        if (firstCellCheck != null && secondCellCheck != null && thirdCellCheck != null)
                        {
                            firstCell = firstCellCheck.ToString();
                            secondCell = secondCellCheck.ToString();
                            thirdCell = thirdCellCheck.ToString();
                        }
                        if (firstCell != "Комната" || secondCell != "Фамилия Имя Отчество" || thirdCell != "Номер телефона")
                        {
                            File.Delete(fileNameExcel);
                            ErrorModel.ErrorMessage = "Таблица подана в не верном формате";
                            ListNewUser.Clear();
                            ListNewRooms.Clear();
                            ListNewUserRooms.Clear();
                            ErrorModel.DialogIsOpen = true;
                            return;
                        }

                        for (int i = sheet.FirstRowNum + 1; i <= sheet.LastRowNum; i++)
                        {
                            UserViewModel userViewModel = new UserViewModel();
                            IRow row = sheet.GetRow(i);
                            if (row == null) { continue; }
                            string deportament = null;
                            string course = null;
                            string phoneNumber = "";
                            string group = null;
                            string room = null;
                            string[] fio = null;

                            string columnLetter = "A";
                            int columnIndex = CellReference.ConvertColStringToIndex(columnLetter);

                            if (row.GetCell(columnIndex) != null)
                            {
                                room = row.GetCell(columnIndex).ToString();
                            }
                            if (room == "" || room == null) continue;

                            if (RoomModel.Any(x => x.NumberRoom == room))
                            {
                                RoomNumber = RoomModel.Where(x => x.NumberRoom == room).FirstOrDefault().NumberRoom;
                            }

                            else
                            {
                                RoomViewModel RoomViewModel = new RoomViewModel();
                                RoomViewModel.NumberRoom = room;
                                RoomViewModel.RoomTypeId = 1;                                
                                RoomViewModel.Floor = int.Parse(room.ToString()[0].ToString());
                                if (RoomViewModel.Floor != 4)
                                {
                                    var check = int.Parse(room.ToString().Substring(1, 2));
                                    if (check < 25)
                                    {
                                        RoomViewModel.Wing = "l";
                                    }
                                    else
                                    {
                                        RoomViewModel.Wing = "r";
                                    }
                                }
                                else
                                {
                                    var check = int.Parse(room.ToString().Substring(1, 2));
                                    if (check < 32|| check==65|| check==67|| check==68)
                                    {
                                        RoomViewModel.Wing = "r";
                                    }
                                    else
                                    {
                                        RoomViewModel.Wing = "l";
                                    }
                                }
                                RoomModel.Add(RoomViewModel);
                                ListNewRooms.Add(RoomViewModel);
                                RoomNumber = RoomViewModel.NumberRoom;//????
                            }

                            columnLetter = "C";
                            columnIndex = CellReference.ConvertColStringToIndex(columnLetter);
                            if (row.GetCell(columnIndex) != null)
                            {
                                fio = (row.GetCell(columnIndex).ToString()).Split(' ', StringSplitOptions.RemoveEmptyEntries);
                            }
                            if (fio == null || fio.Any() == false || fio[0] == "") continue;
                            string name = fio[1];
                            string surname = fio[0];
                            string secondname = null;
                            if (fio.Count() > 2)
                            {
                                secondname = fio[2];
                            }
                            columnLetter = "H";
                            columnIndex = CellReference.ConvertColStringToIndex(columnLetter);
                            if (row.GetCell(columnIndex) != null)
                            {
                                phoneNumber = row.GetCell(columnIndex).ToString();
                            }
                            columnLetter = "D";
                            columnIndex = CellReference.ConvertColStringToIndex(columnLetter);
                            if (row.GetCell(columnIndex) != null)
                            {
                                deportament = row.GetCell(columnIndex).ToString();
                            }

                            columnLetter = "E";
                            columnIndex = CellReference.ConvertColStringToIndex(columnLetter);
                            if (row.GetCell(columnIndex) != null)
                            {
                                course = row.GetCell(columnIndex).ToString();
                            }
                            columnLetter = "G";
                            columnIndex = CellReference.ConvertColStringToIndex(columnLetter);
                            if (row.GetCell(columnIndex) != null)
                            {
                                group = row.GetCell(columnIndex).ToString();
                            }
                            userViewModel.Name = name;
                            userViewModel.Surname = surname;
                            userViewModel.PhoneNumber = phoneNumber;
                            userViewModel.Secondname = secondname;
                            userViewModel.UserCourse = course;
                            userViewModel.UserGroup = group;
                            userViewModel.UserDeportament = deportament;
                            userViewModel.UserRoomNumber = RoomNumber;

                            //var updatedUser = UserModel.FirstOrDefault(x=>) апдейт?
                            if (UserModel.Any(x => x.Name == userViewModel.Name && x.Surname == userViewModel.Surname &&
                            x.PhoneNumber == userViewModel.PhoneNumber && x.Secondname == userViewModel.Secondname))
                            {
                                //если жилец есть, то он заселен, возможно нужна доп проверка на зеселение?
                            }
                            else
                            {
                                ListNewUser.Add(userViewModel);
                            }
                            File.Delete(fileNameExcel);
                            if (ListNewUser.Count !=0)
                            {
                                flag = true;
                                flagForProcessing = true;
                            }
                            else
                            {
                                Snackbar.Add("Новых жильцов не добавлено", Severity.Success);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                LogService.Create(LogModel, ex.Message, ex.StackTrace, DateTime.Now);
            }
        }

        public void LoadFiles()
        {
            try
            {
                Js.InvokeVoidAsync("clickInputFile");
            }
            catch (Exception ex)
            {
                LogService.Create(LogModel, ex.Message, ex.StackTrace, DateTime.Now);
            }
        }
        public void ProcessExcelFile()
        {
            foreach (var item in ListNewRooms)
            {
                item.PeopleMax = 2;
                RoomService.Create(item);
            }
            foreach (var item in ListNewUser)
            {
                var existrooms = RoomService.Get();
                var rezult = UserService.Create(item);
                UserRoomViewModel userRoomViewModel = new UserRoomViewModel();
                userRoomViewModel.UserId = rezult.UserId;
                userRoomViewModel.RoomId = existrooms.FirstOrDefault(x => x.NumberRoom == item.UserRoomNumber).RoomId;
                ListNewUserRooms.Add(userRoomViewModel);
            }

            foreach (var item in ListNewUserRooms)
            {
                UserRoomService.Create(item);
            }
            Snackbar.Add("Таблицы загружены в базу данных", Severity.Success);
            flagForProcessing = false;
        }
        public void ClearTables()
        {
            try
            {
                ListNewUser.Clear();
                ListNewRooms.Clear();
                ListNewUserRooms.Clear();
                TeleService.Clear();
                UserRoomService.Clear();
                RoomService.Clear();
                UserService.Clear();
                Snackbar.Add("Таблицы очищены", Severity.Success);
                flagForProcessing = false;
                flag = false;
            }
            catch (Exception ex)
            {
                LogService.Create(LogModel, ex.Message, ex.StackTrace, DateTime.Now);
            }
        }
        public void ConfirmClear()
        {
            try
            {
                ConfirmModel.DialogIsOpen = true;
            }
            catch (Exception ex)
            {
                LogService.Create(LogModel, ex.Message, ex.StackTrace, DateTime.Now);
            }
        }
    }
}
