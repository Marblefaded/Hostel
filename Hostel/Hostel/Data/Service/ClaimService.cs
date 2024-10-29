using HostelDB.AlfaPruefungDb;
using HostelDB.DbRepository;
using HostelDB.Model;
using Suo.Admin.Data.ViewModel;

namespace Suo.Admin.Data.Service
{
    public class ClaimService
    {
        EFRepository<Claim> repoClaim;
        private HostelDbContext DbContext;

        public ClaimService(HostelDbContext context)
        {
            repoClaim = new EFRepository<Claim>(context);
            DbContext = context;
        }
        public List<ClaimViewModel> Get()
        {
            var list = repoClaim.Get().ToList();
            var result = list.Select(Convert).ToList();
            return result;
        }
        public async Task<List<ClaimViewModel>> GetAll()
        {
            var listItems = repoClaim.Get().ToList();
            var result = listItems.Select(x => Convert(x)).ToList();
            return await Task.FromResult(result);
        }

        private static ClaimViewModel Convert(Claim r)
        {
            var item = new ClaimViewModel(r);
            return item;
        }

        public ClaimViewModel ReloadItem(ClaimViewModel item)
        {
            var x = repoClaim.Reload(item.ClaimTemplateId);
            if (x == null)
            {
                return null;
            }
            return Convert(x);
        }

        public void Delete(ClaimViewModel item)
        {
            var x = repoClaim.FindById(item.ClaimId);
            repoClaim.Remove(x);
        }

        public ClaimViewModel Update(ClaimViewModel item)
        {
            var x = repoClaim.FindByIdForReload(item.ClaimId);

            x.ClaimTypeId = item.ClaimTypeId;
            x.Status = item.Status;
            x.ClaimJson = item.ClaimJson;
            x.ChangeLog = item.ChangeLog;
            x.ClaimTemplateId = item.ClaimTemplateId;
            x.UserId = item.UserId;
            x.CreateDate= item.CreateDate;
            x. DataClaim = item.DataClaim;

            var result = Convert(repoClaim.Update(x, item.Item.RowVersion));
            return result;
        }      

        public ClaimViewModel UpdateStatus(ClaimViewModel item)
        {
            var newItem = repoClaim.Update(item.Item);
            return Convert(newItem);
        }

        public ClaimViewModel Create(ClaimViewModel item)
        {
            var newItem = repoClaim.Create(item.Item);
            return Convert(newItem);
        }
        public List<ClaimViewModel> GetFiltering(DateTime? createDate)
        {
            //var list = DbContext.GetFilteringDate(createDate);
            if (createDate == null)
            {
                var list2 = DbContext.DbSetClaim.AsQueryable().ToList();
                var result2 = list2.Select(Convert).ToList();
                return result2;

            }
            var list = DbContext.DbSetClaim.AsQueryable().Where(x => x.CreateDate.Date == createDate.GetValueOrDefault().Date).Select(Convert).ToList();
            return list;
        }
    }
}
