using JWTCoding;
using Microsoft.AspNetCore.Mvc;
using Suo.TelegramBotSeparated.Models;
using Suo.TelegramBotSeparated.Services;
using System.Diagnostics;

using Suo.TelegramBotSeparated.Services.TelegramBotService;

namespace Suo.TelegramBotSeparated.Controllers
{
    public class HomeController : Controller
    {
        protected TelegramBotSeparate _bot;
        protected DutyForTomorowMesageGeneratorService _dutyForTomorowMesageGeneratorService;

        public HomeController(TelegramBotSeparate bot, DutyForTomorowMesageGeneratorService dutyForTomorowMesageGeneratorService)
        {
            _bot = bot;
            _dutyForTomorowMesageGeneratorService = dutyForTomorowMesageGeneratorService;
        }

        public async Task<IActionResult> IndexAsync()
        {
            var telegramUsers = _dutyForTomorowMesageGeneratorService.ListDutyUsersForTomorow();
            foreach (var user in telegramUsers) 
            {
                _bot.SendDutyMesage(user);
            }
            
            return View();
        }

        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}




