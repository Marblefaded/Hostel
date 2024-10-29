//using Hostel.TelegramBot;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Components;
//using Microsoft.AspNetCore.Mvc;
//using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

//namespace Hostel.Controllers
//{
//    [AllowAnonymous]
//    [Route("api/[controller]/")]
//    [ApiController]
//    public class TelegramBotController : ControllerBase
//    {
//        public string Key { get; set; } = "1aB@4cD#7eF^0gH)2iJ!5kLmNpQrSt";
//        [Inject] protected TelegramBotNotification Bot { get; set; }

//        [HttpGet]
//        //[Route("StartBot/{key}")]
//        public async Task<bool> StartBot(/*string key*/)
//        {
//           await Bot.Start();
//           return true;
//        }

//    }
//}
