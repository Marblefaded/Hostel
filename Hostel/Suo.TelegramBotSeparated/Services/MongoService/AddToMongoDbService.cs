using MongoDB.Driver;
using Suo.TelegramBotSeparated.Models;
using Suo.TelegramBotSeparated.Services;
using Telegram.Bot.Types;

namespace Suo.TelegramBotSeparated.Services.MongoService
{
    public class AddToMongoDbService
    {
        private MongoClient _mongoClient;
        private IMongoDatabase _mongoDb;
        private IMongoCollection<TelegramMessageViewModel> _mongoCollection;
        private readonly LogApplicationService _logService;

        public AddToMongoDbService(LogApplicationService logService, TelegramConfiguration telegramConfig)
        {
            _mongoClient = new MongoClient(telegramConfig.MongoDbString);
            _mongoDb = _mongoClient.GetDatabase("MesageLog");
            _mongoCollection = _mongoDb.GetCollection<TelegramMessageViewModel>("Mesages");
            _logService = logService;

        }

        public async Task AddFromMessageToMongoDbAsync(Update update)
        {
            try
            {
                var mesageLog = new TelegramMessageViewModel
                {
                    Direction = "from",
                    User = $"{update.Message.From.FirstName} {update.Message.From.LastName}",
                    TextMesage = update.Message.Text,
                    DateSend = $"{DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}"
                };
                await _mongoCollection.InsertOneAsync(mesageLog);
            }
            catch (Exception ex)
            {
                _logService.Create(ex.Message, ex.StackTrace, DateTime.Now);
            }
        }

        public async Task AddToMessageToMongoDbAsync(Update update, string text)
        {
            try
            {
                var mesageLog = new TelegramMessageViewModel
                {
                    Direction = "to",
                    User = $"{update.Message.From.FirstName} {update.Message.From.LastName}",
                    TextMesage = text,
                    DateSend = $"{DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}"
                };
                await _mongoCollection.InsertOneAsync(mesageLog);
            }
            catch (Exception ex)
            {
                _logService.Create(ex.Message, ex.StackTrace, DateTime.Now);
            }
        }
        public async Task AddNoticeMessageToMongoDbAsync(string userName, string text)
        {
            try
            {
                var mesageLog = new TelegramMessageViewModel
                {
                    Direction = "to",
                    User = $"{userName}",
                    TextMesage = text,
                    DateSend = $"{DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}"
                };
                await _mongoCollection.InsertOneAsync(mesageLog);
            }
            catch (Exception ex)
            {
                _logService.Create(ex.Message, ex.StackTrace, DateTime.Now);
            }
        }


    }
}
