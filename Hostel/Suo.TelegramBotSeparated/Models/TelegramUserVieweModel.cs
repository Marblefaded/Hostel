using HostelDB.Model;

namespace Suo.TelegramBotSeparated.Models
{
    public class TelegramUserVieweModel : ICloneable
    {
        private TelegramUser _item;
        public TelegramUser Item => _item;
        public TelegramUserVieweModel(TelegramUser item)
        {
            _item = item;
        }
        public TelegramUserVieweModel()
        {
            _item = new TelegramUser();
        }
        public int Id
        {
            get => _item.Id;
            set => _item.Id = value;
        }

        public string TelegramUserId
        {
            get => _item.TelegramUserId;
            set => _item.TelegramUserId = value;
        }
        public string TelegramUserFirstName
        {
            get => _item.TelegramUserFirstName;
            set => _item.TelegramUserFirstName = value;
        }
        public string? TelegramUserLastName
        {
            get => _item.TelegramUserLastName;
            set => _item.TelegramUserLastName = value;
        }
        public int UserId
        {
            get => _item.UserId;
            set => _item.UserId = value;
        }
        
        public object Clone()
        {
            TelegramUserVieweModel item = (TelegramUserVieweModel)this.MemberwiseClone();
            item._item = (TelegramUser)_item.Clone();
            return item;
        }
    }
}
