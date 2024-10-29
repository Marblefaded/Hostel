using Azure;
using HostelDB.AlfaPruefungDb;
using HostelDB.DbRepository;
using HostelDB.DbRepository.Models;
using HostelDB.Model;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.Formula.Functions;
using Suo.Admin.Data.ViewModel;
using static MudBlazor.CategoryTypes;

namespace Suo.Admin.Data.Service
{
    public class DutyService
    {
        EFRepository<Duty> repoDuty;
        private HostelDbContext DbContext;

        public DutyService(HostelDbContext context)
        {
            DbContext = context;
            repoDuty = new EFRepository<Duty>(context);
        }
        public List<DutyViewModel> Get()
        {
            var list = repoDuty.Get().ToList();
            var result = list.Select(Convert).ToList();
            return result;
        }
        public Duty ReverseConvert(DutyViewModel rez)
        {
            Duty model = new Duty();
            model.DutyMonthId = rez.DutyMonthId;
            model.RoomNumber = rez.RoomNumber;
            model.Date = rez.Date;
            model.Floor = rez.Floor;
            return model;
        }
        private DutyViewModel Convert(Duty rez)
        {
            var item = new DutyViewModel(rez);
            return item;
        }
        public DutyViewModel Create(DutyViewModel model)
        {
            var newitem = repoDuty.Create(model.Item);
            return Convert(newitem);
        }


        public DutyViewModel Update(DutyViewModel model)
        {
            var updateitem = repoDuty.Update(model.Item);
            return Convert(updateitem);
        }
        public DutyViewModel Remove(DutyViewModel model)
        {
            repoDuty.Remove(model.Item);
            return null;
        }
        public DutyViewModel FindById(DutyViewModel item)
        {
            return Convert(repoDuty.FindById(item.DutyMonthId));
        }

        public async Task<bool> IsAnyDutys(int floor, int mount, int year, string wing = "")
        {
            var list = repoDuty.GetQuery().Where(x => x.Date.Month == mount && x.Date.Year == year && x.Floor == floor);

            if (!string.IsNullOrEmpty(wing))
            {
                list = list.Where(x => x.Wing == wing);
            }

            return list.Any();

        }

        public List<DutyViewModel> GetForDateFilter(int floor, int mount, int year, string wing = "")
        {
            var list = repoDuty.GetQuery().Where(x => x.Date.Month == mount && x.Date.Year == year && x.Floor == floor);

            if (!string.IsNullOrEmpty(wing))
            {
                list = list.Where(x => x.Wing == wing);
            }

            list = list.OrderBy(x => x.Date);

            var query = from d in list
                        join room in DbContext.DbSetRoom on d.RoomNumber equals room.NumberRoom
                        select new DutyViewModel(d)
                        {
                            RoomId = room.RoomId,
                        };

            return query.AsNoTracking().ToList();

        }

        public Dictionary<int, DutyViewModel> ConvertDutyToDayDutyDictionary(List<DutyViewModel> list)
        {
            return list.ToDictionary(x => x.Date.Day, y => y);
        }

        public async Task ClearOldIfExist(int selectedYear, int selectedMount, int floour, string wing)
        {
            if (await IsAnyDutys(floour, selectedMount, selectedYear, wing))
            {
                var list = repoDuty.GetQuery().Where(x => x.Date.Month == selectedMount && x.Date.Year == selectedYear && x.Floor == floour);

                if (!string.IsNullOrEmpty(wing))
                {
                    list = list.Where(x => x.Wing == wing);
                }

                var resultList = list.ToList();

                DbContext.DbSetDuty.AttachRange(resultList);
                DbContext.DbSetDuty.RemoveRange(resultList);
                DbContext.SaveChanges();

                //foreach (var duty in resultList)
                //{
                //    repoDuty.Remove(duty);
                //}
            }
        }

        public async Task Create(List<Duty> model)
        {


            await DbContext.DbSetDuty.AddRangeAsync(model);
            await DbContext.SaveChangesAsync();

            foreach (var duty in model)
            {
                DbContext.Entry(duty).State = EntityState.Detached;
            }
            DbContext.SaveChanges();

        }

    }
}
