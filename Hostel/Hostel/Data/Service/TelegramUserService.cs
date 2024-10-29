using HostelDB.AlfaPruefungDb;
using HostelDB.DbRepository;
using HostelDB.Model;
using NPOI.SS.Formula.Functions;
using Suo.Admin.Data.ViewModel;

namespace Suo.Admin.Data.Service
{
    public class TelegramUserService
    {
        EFRepository<TelegramUser> _repoTelegramUser;
        private HostelDbContext _dbContext;

        public TelegramUserService(HostelDbContext context)
        {
            _repoTelegramUser = new EFRepository<TelegramUser>(context);
            _dbContext = context;
        }
       
        public List<TelegramUserVieweModel> Get()
        {
            var list = _repoTelegramUser.Get().ToList();
            var result = list.Select(Convert).ToList();
            return result;
        }
        private TelegramUserVieweModel Convert(TelegramUser rez)
        {
            var item = new TelegramUserVieweModel(rez);
            return item;
        }
       
        public TelegramUserVieweModel Remove(TelegramUserVieweModel model)
        {
            _repoTelegramUser.Remove(model.Item);
            return null;
        }
     
        public void Clear()
        {
            _dbContext.ClearTeleUsers();
        }

        public TelegramUser UserInfo(int userId)
        {
            var telegramUser = _repoTelegramUser.GetQuery().FirstOrDefault(x => x.UserId == userId && !string.IsNullOrEmpty(x.TelegramUserId));
            return telegramUser;
        }
    }
}
