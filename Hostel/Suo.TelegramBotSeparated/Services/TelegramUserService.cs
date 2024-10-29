using HostelDB.AlfaPruefungDb;
using HostelDB.DbRepository;
using HostelDB.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Suo.TelegramBotSeparated.Models;

namespace Suo.TelegramBotSeparated.Services
{
    public class TelegramUserService
    {
        EFRepository<TelegramUser> _repo;
        EFRepository<User> _userRepository;
        EFRepository<AmsUser> _amsuserRepository;
        HostelDbContext _DbContext;

        public TelegramUserService(HostelDbContext context)
        {
            _repo = new EFRepository<TelegramUser>(context);
            _userRepository = new EFRepository<User>(context);
            _amsuserRepository = new EFRepository<AmsUser>(context);
            _DbContext = context;
        }

        public bool CheckExist(string phonenumber)
        {
            var list = _userRepository.Get().ToList();
            if (list.Any(x => x.PhoneNumber == phonenumber))
            {
                return true;
            }
            else return false;
        }
        public int CheckDuble(string phonenumber)
        {
            var listuser = _userRepository.Get().ToList();
            var listamsuser = _amsuserRepository.Get().ToList();
            var userId = listuser.FirstOrDefault(x => x.PhoneNumber == phonenumber);

            if (listamsuser.Any(x => x.UserId == userId.UserId) == true)
            {
                return -1;
            }

            else return userId.UserId;
        }


        public List<TelegramUserVieweModel> Get()
        {
            var list = _repo.Get().ToList();
            var result = list.Select(Convert).ToList();
            return result;
        }

        public TelegramUserVieweModel FindByTelegrammChatId(string chatId)
        {
            var telegramUser = _repo.GetQuery().FirstOrDefault(x => x.TelegramUserId == chatId && !string.IsNullOrEmpty(x.TelegramUserFirstName));
            return Convert(telegramUser);           
        }
        private TelegramUserVieweModel Convert(TelegramUser rez)
        {
            var item = new TelegramUserVieweModel(rez);
            return item;
        }
        public TelegramUserVieweModel Create(TelegramUserVieweModel model)
        {
            var newitem = _repo.Create(model.Item);
            return Convert(newitem);
        }  
    }
}

