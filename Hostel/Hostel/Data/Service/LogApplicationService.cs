using HostelDB.AlfaPruefungDb;
using HostelDB.DbRepository;
using HostelDB.Model;
using System.Reflection;
using Suo.Admin.Data.ViewModel;
using Microsoft.AspNetCore.Components.Authorization;
using Suo.Admin.Data.Services;
using Microsoft.IdentityModel.Tokens;

namespace Suo.Admin.Data.Service
{
    public class LogApplicationService
    {
        EFRepository<LogMessageEntry> repoLog;
        private HostelDbContext DbContext;
        private HostelAuthenticationStateProvider _hostelAuthenticationStateProvider;


        public LogApplicationService(HostelDbContext context, AuthenticationStateProvider authenticationStateProvider)
        {
            _hostelAuthenticationStateProvider = ((HostelAuthenticationStateProvider)authenticationStateProvider);
            repoLog = new EFRepository<LogMessageEntry>(context);
            DbContext = context;
        }
        public async Task<List<LogApplicationViewModel>> GetAll()
        {
            var listItems = repoLog.Get();
            var result = listItems.Select(x => Convert(x)).ToList();
            result.Reverse();
            return await Task.FromResult(result);
        }

        private static LogApplicationViewModel Convert(LogMessageEntry r)
        {
            var item = new LogApplicationViewModel(r);
            return item;
        }

        public LogApplicationViewModel ReloadItem(LogApplicationViewModel item)
        {
            var x = repoLog.Reload(item.LogApplicationId);
            if (x == null)
            {
                return null;
            }
            return Convert(x);
        }

        /*public void DeleteSelected()
        {
            var itemsToDelete = DbContext.DbSetLogApplication.Where(x => x.IsEnable == true);
            DbContext.DbSetLogApplication.RemoveRange(itemsToDelete);
            DbContext.SaveChanges();
        }*/

        public void Delete(LogApplicationViewModel item)
        {
            var x = repoLog.FindById(item.LogApplicationId);
            repoLog.Remove(x);
        }

        public LogApplicationViewModel Create(LogApplicationViewModel item, string msg, string stackTrace, DateTime date)
        {
            var version = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
            var name = _hostelAuthenticationStateProvider.AuthenticationStateUser.Identity.Name;
            var listSurname = _hostelAuthenticationStateProvider.AuthenticationStateUser.Identities.FirstOrDefault().Claims.Where(x => x.Type.Contains("surname")).ToList();
            item = new LogApplicationViewModel();
            item.Date = date;
            item.Message = msg;
            item.ErrorContext = stackTrace;
            item.AppVersion = version;
            if (!name.IsNullOrEmpty())
            {
                item.UserName = $"{name} {item.UserName = listSurname.FirstOrDefault().Value}";
            }
            else
            {
                item.UserName = "";
            }
            Console.WriteLine($"\n{msg}   {stackTrace}  {date}");
            var newItem = repoLog.Create(item.Item);
            return Convert(newItem);
        }

        public LogApplicationViewModel Update(LogApplicationViewModel item)
        {
            var x = repoLog.FindById(item.LogApplicationId);
            x.ErrorMsg = item.Message;
            x.ErrorContext = item.ErrorContext;
            x.UserName = item.UserName;
            x.InsertDate = item.Date;
            x.IsDeleted = item.IsDeleted;
            /*x.IsEnable = item.IsEnable;*/
            return Convert(repoLog.Update(x));
        }

        public void DeleteAllLogs()
        {
            DbContext.DbSetLogApplication.RemoveRange(DbContext.DbSetLogApplication);
            DbContext.SaveChanges();
        }

        public List<LogApplicationViewModel> Filtering(DateTime? y)
        {
            var filteredListLogs = repoLog.GetQuery().Where(x => x.InsertDate.Date == y.GetValueOrDefault().Date).ToList();
            var result = filteredListLogs.Select(Convert).ToList();
            result.Reverse();
            return result;
        }
        public List<LogApplicationViewModel> FilteringError(string message)
        {
            var filteredListLogs = repoLog.GetQuery().Where(x => (x.ErrorMsg.Contains(message))).ToList();
            var result = filteredListLogs.Select(Convert).ToList();
            result.Reverse();
            return result;
        }
    }
}
