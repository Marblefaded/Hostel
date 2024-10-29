using HostelDB.AlfaPruefungDb;
using HostelDB.DbRepository;
using HostelDB.Model;
using Suo.Admin.Data.ViewModel;

namespace Suo.Admin.Data.Service
{
    public class ClaimTemplateService
    {
        EFRepository<ClaimTemplate> repoClaimTemp;
        private HostelDbContext DbContext;

        public ClaimTemplateService(HostelDbContext context)
        {
            repoClaimTemp = new EFRepository<ClaimTemplate>(context);
            DbContext = context;
        }

        public List<ClaimTemplateViewModel> Get()
        {
            var list = repoClaimTemp.GetQuery().ToList();
            var result = list.Select(Convert).ToList();
            return result;
        }
        public async Task<List<ClaimTemplateViewModel>> GetAll()
        {
            var listItems = repoClaimTemp.Get();
            var result = listItems.Select(x => Convert(x)).ToList();
            foreach (var item in result)
            {
                item.IsDeleteEnabled = DbContext.IsClaimDeleteEnabled(item.ClaimTemplateId);
            }
            return await Task.FromResult(result);
        }

        private static ClaimTemplateViewModel Convert(ClaimTemplate r)
        {
            var item = new ClaimTemplateViewModel(r);
            return item;
        }

        public ClaimTemplateViewModel ReloadItem(ClaimTemplateViewModel item)
        {
            var x = repoClaimTemp.Reload(item.ClaimTemplateId);
            if (x == null)
            {
                return null;
            }
            return Convert(x);
        }

        public void Delete(ClaimTemplateViewModel item)
        {
            var x = repoClaimTemp.FindById(item.ClaimTemplateId);
            repoClaimTemp.Remove(x);
        }

        public ClaimTemplateViewModel Update(ClaimTemplateViewModel item)
        {
            var x = repoClaimTemp.FindByIdForReload(item.ClaimTemplateId);

            x.Title = item.Title;
            x.ClaimJson = item.ClaimJson;
            x.ClaimTypeId = item.ClaimTypeId;
            x.IsActive = item.IsActive;
            x.ChangeLog = item.ChangeLog;
            x.TemplateModelJson = item.TemplateModelJson;
            x.DataTemplate = item.DataTemplate;

            var result = Convert(repoClaimTemp.Update(x, item.Item.RowVersion));

            return result;
        }

        public ClaimTemplateViewModel Create(ClaimTemplateViewModel item)
        {
            var newItem = repoClaimTemp.Create(item.Item);
            return Convert(newItem);
        }
    }
}
