using HostelDB.Model;
using System.ComponentModel.DataAnnotations;

namespace Suo.TelegramBotSeparated.Models
{
    public class LogApplicationViewModel
    {
        private LogMessageEntry _item;
        public LogMessageEntry Item => _item;

        public LogApplicationViewModel()
        {
            _item = new LogMessageEntry();

        }

        public LogApplicationViewModel(LogMessageEntry item)
        {
            _item = item;
        }

        [Key]
        public int LogApplicationId
        {
            get => _item.LogApplicationId;
            set => _item.LogApplicationId = value;
        }

        public string Message
        {
            get => _item.ErrorMsg;
            set => _item.ErrorMsg = value;
        }
        public string ErrorContext
        {
            get => _item.ErrorContext;
            set => _item.ErrorContext = value;
        }
        public string UserName
        {
            get => _item.UserName;
            set => _item.UserName = value;
        }
        public DateTime Date
        {
            get => _item.InsertDate;
            set => _item.InsertDate = value;
        }
        public bool IsDeleted
        {
            get => _item.IsDeleted;
            set => _item.IsDeleted = value;
        }
        public bool IsEnableDelete
        {
            get;
            set;
        }
        public string AppVersion
        {
            get => _item.AppVersion;
            set => _item.AppVersion = value;
        }

    }
}
