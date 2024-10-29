using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using NPOI.XWPF.UserModel;
using NPOIPdfEngine.Models;
using Suo.Admin.Data.EditModel;
using Suo.Admin.Data.RabbitMqService;
using Suo.Admin.Data.Service;
using Suo.Admin.Data.ViewModel;
using Suo.Admin.Shared;
using Document = Spire.Doc.Document;

namespace Suo.Admin.Pages.AllApplications
{
    public class AllApplicationsView : ComponentBase
    {
        [Inject] protected IDialogService DialogService { get; set; }
        [Inject] protected IJSRuntime jsRunTime { get; set; }
        [Inject] protected UserService UserService { get; set; }
        [Inject] protected TelegramUserService TelegramUserService { get; set; }
        [Inject] protected RabbitService RabbitService { get; set; }
        [Inject] protected ClaimService ClaimService { get; set; }
        [Inject] protected ClaimTemplateService TemplateService { get; set; }
        [Inject] private LogApplicationService LogService { get; set; }
        public LogApplicationViewModel LogModel = new LogApplicationViewModel();
        protected List<UserViewModel> UserModel { get; set; } = new();
        protected List<ClaimTemplateViewModel> TemplateModel { get; set; } = new();
        protected List<ClaimViewModel> Model { get; set; } = new();

        public ClaimViewModel mCurrentItem;
        public EditClaimItemViewModel mEditViewModel = new EditClaimItemViewModel();

        public bool isRemove;
        public bool isOpen;
        public ApplicationView modalView { get; set; }
        public DateTime? filteringDate { get; set; }


        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            try
            {
                if (firstRender)
                {
                    FilteringDate = DateTime.Now;
                    UserModel = UserService.Get();
                    TemplateModel = TemplateService.Get();
                    await InvokeAsync(StateHasChanged);
                }
            }
            catch (Exception ex)
            {
                LogService.Create(LogModel, ex.Message, ex.StackTrace, DateTime.Now);
            }
        }


        public DateTime? FilteringDate
        {
            get { return filteringDate; }
            set { filteringDate = value; Search(); }
        }

        public void Search()
        {
            try
            {
                Model = ClaimService.GetFiltering(filteringDate);
            }
            catch (Exception ex)
            {
                LogService.Create(LogModel, ex.Message, ex.StackTrace, DateTime.Now);
            }
        }
        //public string GetName(int userId)
        //{
        //    try
        //    {
        //        return UserService.GetFIO(userId);
        //    }
        //    catch (Exception ex)
        //    {
        //        LogService.Create(LogModel, ex.Message, ex.StackTrace, DateTime.Now);
        //        return null;
        //    }
        //}

        //public async void EditItem(ClaimViewModel item)
        //{
        //    try
        //    {
        //        mEditViewModel.Models = await ClaimService.GetAll();
        //        mEditViewModel.Item = item;
        //        mCurrentItem = item;
        //        mEditViewModel.IsOpened = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        LogService.Create(LogModel, ex.Message, ex.StackTrace, DateTime.Now);
        //    }
        //}
        public async Task DeleteItemAsync(ClaimViewModel item)
        {
            try
            {
                var dialog = DialogService.Show<DeleteComponent>("Вы точно хотите удалить это заявление?");
                var result = await dialog.Result;
                if (!result.Canceled)
                {
                    ClaimService.Delete(item);
                    Model.Remove(item);
                }
                StateHasChanged();
            }
            catch (Exception ex)
            {
                LogService.Create(LogModel, ex.Message, ex.StackTrace, DateTime.Now);
            }
        }

        public async Task OpenFileDialogAsync(ClaimViewModel item2)
        {
            try
            {
                var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Medium };
                var parameters = new DialogParameters<ApplicationView> { { x => x.mCurrentItem, item2 } };
                var dialog = DialogService.Show<ApplicationView>($"Просмотр", parameters, options);
                var result = await dialog.Result;
            }
            catch (Exception ex)
            {
                LogService.Create(LogModel, ex.Message, ex.StackTrace, DateTime.Now);
            }
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

        public async void Change(ClaimViewModel item, bool isApproved)
        {
            try
            {

                if (item.Status == 0)
                {
                    if (isApproved)
                    {
                        item.Status = 1;
                        ClaimService.UpdateStatus(item);
                        var newMesage = new MessageModelForTg() { TelegrammUserId = int.Parse(TelegramUserService.UserInfo(item.UserId).TelegramUserId), Message = "Ваше заявление успешно принято" };
                        RabbitService.SendMessageToTgBot(newMesage);
                    }
                    else
                    {
                        item.Status = 2;
                        ClaimService.UpdateStatus(item);
                        var newMesage = new MessageModelForTg() { TelegrammUserId = int.Parse(TelegramUserService.UserInfo(item.UserId).TelegramUserId), Message = "Ваше заявление отклонено" };
                        RabbitService.SendMessageToTgBot(newMesage);
                    }
                }
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

        public async Task CreateTaskPdf(ClaimViewModel item)
        {
            try
            {
                mCurrentItem = item;

                var newFile = @"D:\Obshaga-update.docx";
                using (var fs = new FileStream(newFile, FileMode.Create, FileAccess.Write))
                {
                    XWPFDocument doc = new XWPFDocument();
                    var probel0 = doc.CreateParagraph();
                    var p0 = doc.CreateParagraph();
                    p0.Alignment = ParagraphAlignment.RIGHT;
                    XWPFRun r0 = p0.CreateRun();

                    var Rank = doc.CreateParagraph();
                    Rank.Alignment = ParagraphAlignment.RIGHT;
                    XWPFRun ranc = Rank.CreateRun();
                    ranc.FontFamily = "times new roman";
                    ranc.FontSize = 12;
                    ranc.SetText("Заведующей общежитием №1");

                    var p = doc.CreateParagraph();
                    p.Alignment = ParagraphAlignment.RIGHT;
                    XWPFRun r = p.CreateRun();
                    r.FontFamily = "times new roman";
                    r.FontSize = 12;
                    /*r.SetText("Коменданту общежития №1\n" + "Жамьянову И.Ю");*/
                    r.SetText("Суворовой В. В.");



                    var probel1 = doc.CreateParagraph();
                    var probel2 = doc.CreateParagraph();
                    var probel3 = doc.CreateParagraph();
                    var probel4 = doc.CreateParagraph();
                    var probel5 = doc.CreateParagraph();
                    var probel6 = doc.CreateParagraph();
                    var probel7 = doc.CreateParagraph();
                    var probel8 = doc.CreateParagraph();
                    var probel9 = doc.CreateParagraph();
                    var probel10 = doc.CreateParagraph();
                    var probel11 = doc.CreateParagraph();

                    var p1 = doc.CreateParagraph();
                    p1.Alignment = ParagraphAlignment.CENTER;
                    XWPFRun r1 = p1.CreateRun();
                    r1.FontFamily = "times new roman";
                    r1.FontSize = 16;
                    r1.IsBold = true;
                    r1.SetText($"Прибытие студентов после 23:00 от {filteringDate.Value.Date.ToString("d")}");
                    var x = TemplateModel.FirstOrDefault(x => x.Title == "Заявление на возвращение после 23:00").ClaimTemplateId;

                    foreach (var i in Model.Where(x => x.Status == 1 || x.Status == 0).ToList())
                    {
                        mCurrentItem = i;
                        if (mCurrentItem.CreateDate.Day == filteringDate.Value.Day)
                        {
                            if (mCurrentItem.CreateDate.Month == filteringDate.Value.Month && mCurrentItem.ClaimTemplateId == x)
                            {
                                var probel12 = doc.CreateParagraph();

                                var p2 = doc.CreateParagraph();
                                p2.Alignment = ParagraphAlignment.LEFT;
                                p2.IndentationFirstLine = 500;
                                XWPFRun r2 = p2.CreateRun();
                                r2.FontFamily = "times new roman";
                                r2.FontSize = 12;
                                ReportModel rModel = new ReportModel();
                                rModel = System.Text.Json.JsonSerializer.Deserialize<ReportModel>(mCurrentItem.ClaimJson);
                                var nextDay = DateTime.Parse(rModel.Dateofapplication).AddDays(1);
                                r2.SetText($"{rModel.Lastname} {rModel.Firstname} {rModel.Middlename} - {rModel.TimeToGo} {nextDay.ToString("dd.MM.yyyy")}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("null");
                        }
                    }
                    //doc.Write(fs);

                    MemoryStream memory = new MemoryStream();

                    doc.Write(memory);
                    doc.Close();
                    //memory.Position = 0;
                    MemoryStream memTemp = new MemoryStream(memory.ToArray());

                    Document document = new Document();
                    document.LoadFromStream(memTemp, Spire.Doc.FileFormat.Docx);
                    memory.Close();
                    memTemp.Close();
                    MemoryStream memoryPdf = new MemoryStream();
                    document.SaveToStream(memoryPdf, Spire.Doc.FileFormat.PDF);
                    //document.SaveToFile(@"D:\Obshaga-pdf.pdf", Spire.Doc.FileFormat.PDF);
                    byte[] bytes = memoryPdf.ToArray();
                    memoryPdf.Close();
                    /*return memoryPdf.ToArray();*/
                    await SaveAsFileAsync(jsRunTime, $"Заявления за {filteringDate.Value.Date.ToString("d")}.pdf", bytes);
                }
            }
            catch (Exception ex)
            {
                LogService.Create(LogModel, ex.Message, ex.StackTrace, DateTime.Now);
            }
        }
    }
}
