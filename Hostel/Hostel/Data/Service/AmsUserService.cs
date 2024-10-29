using HostelDB.AlfaPruefungDb;
using HostelDB.DbRepository;
using Suo.Admin.Data.ViewModel;
using AmsUser = HostelDB.Model.AmsUser;

namespace Suo.Admin.Data.Service
{
    public class AmsUserService
    {
        EFRepository<HostelDB.Model.AmsUser> repoUser;
        private HostelDbContext DbContext;

        public AmsUserService(HostelDbContext context)
        {
            repoUser = new EFRepository<HostelDB.Model.AmsUser>(context);
            DbContext = context;
        }

        public List<AmsUserViewModel> Get()
        {
            var list = repoUser.GetQuery().ToList();
            var result = list.Select(Convert).ToList();
            return result;
        }
        public static AmsUserViewModel Convert(AmsUser r)
        {
            var item = new AmsUserViewModel(r);
            return item;
        }
        public void Delete(AmsUserViewModel item)
        {
            var x = repoUser.GetQuery().ToList();
            var rez = x.Find(x => x.Id == item.Id);
            repoUser.Remove(rez);
        }
    }
}
