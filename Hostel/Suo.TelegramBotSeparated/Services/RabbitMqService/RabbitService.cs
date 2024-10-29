using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Suo.TelegramBotSeparated.Models;
using Suo.TelegramBotSeparated.Services;
using Suo.TelegramBotSeparated.Services.MongoService;
using System.Text;
using System.Text.Json;
using Telegram.Bot;

namespace Suo.TelegramBotSeparated.Services.RabbitMqService
{
    public class RabbitService
    {
        private ConnectionFactory _factory;
        private IConnection _connection;
        private IModel _channel;
        private readonly LogApplicationService _logService;
        private readonly AddToMongoDbService _addToMongoDbService;
        private readonly TelegramUserService _telegramService;
        public RabbitService(LogApplicationService logService, AddToMongoDbService addToMongoDbService, TelegramUserService telegramService, RabbitMQConnectionFactory connectionFactory)
        {
            _factory = connectionFactory.CreateConnectionFactory();
            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();
            _logService = logService;
            _addToMongoDbService = addToMongoDbService;
            _telegramService = telegramService;
        }

        public async Task CreateConsumer(ITelegramBotClient bot)
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                byte[] body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                var deserMesage = JsonSerializer.Deserialize<ConsumedMessageModelForTg>(message);
                var firstname = _telegramService.FindByTelegrammChatId(deserMesage.TelegrammUserId.ToString()).TelegramUserFirstName;
                var lastname = _telegramService.FindByTelegrammChatId(deserMesage.TelegrammUserId.ToString()).TelegramUserLastName;
                if (lastname == null)
                {
                    lastname = "";
                }
                string userName = $"{firstname} {lastname}";
                await _addToMongoDbService.AddNoticeMessageToMongoDbAsync(userName, deserMesage.Message);
                await SendMesage(deserMesage.TelegrammUserId.ToString(), deserMesage.Message, bot);
            };
            _channel.BasicConsume(queue: "suoMesages",
                autoAck: true,
                consumer: consumer);
        }
        public async Task SendMesage(string chatId, string mesage, ITelegramBotClient bot)
        {
            try
            {
                await bot.SendTextMessageAsync(chatId, mesage);
            }
            catch (Exception ex)
            {
                _logService.Create(ex.Message, ex.StackTrace, DateTime.Now);
            }
        }
    }
}
