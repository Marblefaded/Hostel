using GlobalSpecialWords;
using HostelDB.AlfaPruefungDb;
using HostelDB.DbRepository;
using HostelDB.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NPOIPdfEngine;
using NPOIPdfEngine.Models;
using Suo.Client.Attributes;
using Suo.Client.Data.Services;
using Suo.Client.Models;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Suo.Client.Data.Models.Services;
using Suo.Client.Extentions;
using Suo.Client.Data.RabbitMqService;

namespace Suo.Client.Controllers;

[AuthorizeJwt]

public class HomeController : Controller
{

    private int userId = 4;
    private HostelDbContext db;
    private readonly ILogger<HomeController> _logger;
    private EFRepository<HostelDB.Model.Claim> efRepository;
    private EFRepository<ClaimTemplate> eF;

    private LocalStorage _storage;
    private Engine engine;

    private readonly AppConfiguration _config;

    private readonly UserService _userService;
    private readonly IHttpContextAccessor _accessor;
    public ClaimTemplate claimview = new ClaimTemplate();
    public GlobalClaimModel global = new();



    private readonly HttpAuthenticationService _authenticationService;
    private readonly RabbitService _rabbitMqService;

    public HomeController(HostelDbContext context, ILogger<HomeController> logger, Engine reportEngine, AppConfiguration config,
                            UserService userService, IHttpContextAccessor accessor, HttpAuthenticationService authenticationService,
                            RabbitService rabbitMqService)
    {
        efRepository = new EFRepository<HostelDB.Model.Claim>(context);
        eF = new EFRepository<ClaimTemplate>(context);
        _logger = logger;
        db = context;
        this.engine = reportEngine;
        _userService = userService;
        _accessor = accessor;
        _authenticationService = authenticationService;
        _config = config;
        _rabbitMqService = rabbitMqService;
    }

    public async Task<IActionResult> Index()
    {

        //if (DateTime.Now.ToUniversalTime().AddHours(7).Hour > 23 || DateTime.Now.ToUniversalTime().AddHours(7).Hour < 6)
        //{
        //    return Redirect("/Block/Index");
        //}
        var claims = await db.DbSetClaimTemplate.ToListAsync();
        var user = await _userService.GetCurrentUser();
        if (user == null)
        {
            _authenticationService.LogOut(_accessor.HttpContext);
            Redirect("/");
        }
        var model = new HomeItemViewModel()
        {
            User = user,
            Claims = claims.TakeLast(3).ToList(),
            IsEnableTimeToGo = claims.TakeLast(3).ToDictionary(x => x.ClaimTemplateId, y => !string.IsNullOrEmpty(JsonConvert.DeserializeObject<ReportModel>(y.TemplateModelJson).TimeToGo)),
            IsEnableTimeToEnd = claims.TakeLast(3).ToDictionary(x => x.ClaimTemplateId, y => !string.IsNullOrEmpty(JsonConvert.DeserializeObject<ReportModel>(y.TemplateModelJson).TimeToEnd)),
            IsEnableTimeToStart = claims.TakeLast(3).ToDictionary(x => x.ClaimTemplateId, y => !string.IsNullOrEmpty(JsonConvert.DeserializeObject<ReportModel>(y.TemplateModelJson).TimeToStart)),
            IsEnableReason = claims.TakeLast(3).ToDictionary(x => x.ClaimTemplateId, y => !string.IsNullOrEmpty(JsonConvert.DeserializeObject<ReportModel>(y.TemplateModelJson).Reason)),
            Posts = db.GetLastPost(3),
            DateDuty = db.GetNearestDateDutyByRoom(db.GetRoomNumberByUser(user.UserId)),
        };
        if (model.Claims.Count > 0)
        {
            var newList = new List<ClaimTemplate>();
            foreach (var claim in model.Claims)
            {
                switch (claim.Title)
                {
                    case "Заявление на возвращение после 23:00":
                        newList.Add(claim);
                        break;
                    case "Заявление на краткосрочное отсутствие":
                        newList.Insert(0, claim);
                        break;
                    case "Заявление на долгосрочное отсутствие":
                        newList.Insert(0, claim);
                        break;
                    default:
                        break;
                }

                model.Claims = newList;
            }
        }
        return View(model);
    }
  
    private bool IsNullOrEmpty(List<ClaimTemplate> claims)
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    public async Task<IActionResult> Create(int ClaimTemplateId, string ClaimJson, TimeOnly? TimeToGo, DateTime? TimeToStart, DateTime? TimeToEnd, string managerid)
    {
        //var modelSpecs = Globals.GlobalModelSpec;
        var claim = new HostelDB.Model.Claim();
        var claimTemplate = eF.FindById(ClaimTemplateId);
        var userIdS = _accessor.HttpContext.User.FindFirst("UserId")?.Value;
        _storage = new LocalStorage(this.HttpContext);

        if (DateTime.Now.ToUniversalTime().AddHours(7).Hour >= 23 || DateTime.Now.ToUniversalTime().AddHours(7).Hour < 6)
        {
            return Redirect("/Block/Index");
        }

        if (!int.TryParse(userIdS, out userId))
        {
            return Redirect("/Block/Index");
        }

        User user = await _userService.GetUser(userId);

        claim.ClaimTemplateId = claimTemplate.ClaimTemplateId;
        claim.UserId = user.UserId;
        claim.ChangeLog = "create claim";
        claim.CreateDate = DateTime.Now;


        ReportModel rModel = new ReportModel();
        rModel = System.Text.Json.JsonSerializer.Deserialize<ReportModel>(claimTemplate.TemplateModelJson);

        rModel.Facultet = user.UserDeportament;
        rModel.Kurs = user.UserCourse;
        rModel.Group = user.UserGroup;
        rModel.Firstname = $"{user.Name.Substring(0, 1)}.";
        if (user.Secondname != null)
        {
            rModel.Middlename = $"{user.Secondname.Substring(0, 1)}.";
        }
        else
        {
            rModel.Middlename = "";
        }
        rModel.Lastname = user.Surname;
        rModel.Phonenumber = user.PhoneNumber;
        rModel.Dateofapplication = DateTime.Now.ToString("dd.MM.yyyy");
        rModel.Room = db.GetRoomNumber(userId);//!!!!!

        switch (managerid)
        {
            case "1":
                {
                    rModel.NameOfManager = "Лейман А.К.";
                    rModel.RankOfManagement = "Комендант общежития №1";
                    break;
                }
            case "2":
                {
                    rModel.NameOfManager = "Жамьянов И.Ю.";
                    rModel.RankOfManagement = "Комендант общежития №1";
                    break;
                }
            default:
                {
                    rModel.NameOfManager = "Лейман А.К.";
                    rModel.RankOfManagement = "Комендант общежития №1";
                    break;
                }
        }

        if (!string.IsNullOrEmpty(rModel.Reason))
        {
            if (ClaimJson == null)
            {
                Redirect("/");
            }
            rModel.Reason = ClaimJson;
        }

        if (!string.IsNullOrEmpty(rModel.TimeToStart))
        {
            if (TimeToStart == null)
            {
                Redirect("/");
            }
            rModel.TimeToStart = TimeToStart.GetValueOrDefault().ToString("dd.MM.yyyy");
        }


        if (!string.IsNullOrEmpty(rModel.TimeToEnd))
        {
            if (TimeToEnd == null)
            {
                Redirect("/");
            }
            rModel.TimeToEnd = TimeToEnd.GetValueOrDefault().ToString("dd.MM.yyyy");
        }


        if (!string.IsNullOrEmpty(rModel.TimeToGo))
        {
            if (TimeToGo == null)
            {
                Redirect("/");
            }
            rModel.TimeToGo = TimeToGo.ToString();
        }

        /* = db.GetRoomNumber(roomId, "Room", "NumberRoom");*///!!!!!



        claim.ChangeLog = "create template";
        claim.ClaimTypeId = 1;
        //claim.ClaimJson = $@"{claimTemplate.Title}.pdf";
        //ViewModel.report.Student = "[firstname]";

        claim.ClaimJson = System.Text.Json.JsonSerializer.Serialize(rModel);
        var result = engine.CreateReportPdf(rModel);
        claim.DataClaim = result;
        //MudDialog.Close(DialogResult.Ok(claim));
        //await InvokeAsync(StateHasChanged);

        claim.Status = 0;
        efRepository.Create(claim);
     
        var mesageModel = new MessageModelForTg()
        {
            TelegrammUserId = int.Parse(_userService.UserInfo(claim.UserId).TelegramUserId),
            Message = "Ваше заявление успешно отправлено"
        };
        await _rabbitMqService.SendMessageToTgBot(mesageModel);

        return RedirectToAction("Index");
    }

    public IActionResult About()
    {
        var templateList = db.DbSetClaimTemplate.ToList();
        global.claimTemplates = templateList;
        return View(global);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

}



