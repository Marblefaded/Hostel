using HostelDB.AlfaPruefungDb;
using HostelDB.DbRepository;
using HostelDB.Model;
using System.Reflection;
using Microsoft.IdentityModel.Tokens;
using Suo.TelegramBotSeparated.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Telegram.Bot.Types;

namespace Suo.TelegramBotSeparated.Services

{
    public class LogApplicationService
    {
        EFRepository<LogMessageEntry> repoLog;
        private HostelDbContext _DbContext;


        public LogApplicationService(HostelDbContext context)
        {
            repoLog = new EFRepository<LogMessageEntry>(context);
            _DbContext = context;
        }
      

        private static LogApplicationViewModel Convert(LogMessageEntry r)
        {
            var item = new LogApplicationViewModel(r);
            return item;
        }
   
        public LogApplicationViewModel Create(string msg, string stackTrace, DateTime date)
        {
            var version = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
            var item = new LogApplicationViewModel()
            {
                Date = date,
                Message = msg,
                ErrorContext= stackTrace,
                AppVersion = version,
                UserName = "Telegram bot"
            };          
            var newItem = repoLog.Create(item.Item);
            return Convert(newItem);
        }
    }
}
