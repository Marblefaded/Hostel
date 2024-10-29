using HostelDB.AlfaPruefungDb;
using HostelDB.DbRepository;
using HostelDB.Model;
using Suo.TelegramBotSeparated.Models;
using System.Collections.Generic;

namespace Suo.TelegramBotSeparated.Services
{
    public class DutyForTomorowMesageGeneratorService
    {
        readonly EFRepository<Duty> _repoDuty;
        readonly EFRepository<User> _repoUser;
        readonly EFRepository<UserRoom> _repoUserRoom;
        readonly EFRepository<Room> _repoRoom;
        readonly EFRepository<TelegramUser> _repoTelegramUser;
        private readonly HostelDbContext _dbContext;


        public DutyForTomorowMesageGeneratorService(HostelDbContext context)
        {
            _dbContext = context;
            _repoDuty = new EFRepository<Duty>(context);
            _repoUser = new EFRepository<User>(context);
            _repoUserRoom = new EFRepository<UserRoom>(context);
            _repoRoom = new EFRepository<Room>(context);
            _repoTelegramUser = new EFRepository<TelegramUser>(context);
        }

        public DateTime DateToday { get; set; } = DateTime.Now;
    
        public List<TelegramUserVieweModel> ListDutyUsersForTomorow()
        {
            var DutysTomorow = _repoDuty.GetQuery().Where(x => x.Date.Date == DateToday.AddDays(1).Date).ToList();

            var ListNumberRoomTomorow = DutysTomorow.Select(x => x.RoomNumber).ToList();
            var ListRommsIdForRoomTomorow = _repoRoom.GetQuery().Where(x => ListNumberRoomTomorow.Contains(x.NumberRoom)).Select(x => x.RoomId).ToList();
            var ListUsersIdForTomorow = _repoUserRoom.GetQuery().Where(x => ListRommsIdForRoomTomorow.Contains(x.RoomId)).Select(x => x.UserId).ToList();
            var ListMeasgesForTomorow = _repoTelegramUser.GetQuery().Where(x => ListUsersIdForTomorow.Contains(x.UserId)).ToList();
            return ListMeasgesForTomorow.Select(Convert).ToList();
        }

        private TelegramUserVieweModel Convert(TelegramUser rez)
        {
            var item = new TelegramUserVieweModel(rez);
            return item;
        }
    }
}
