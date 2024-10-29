using JWTCoding;
using Microsoft.AspNetCore.Mvc;
using Suo.TelegramBotSeparated.Models;
using Suo.TelegramBotSeparated.Services;
using System.Diagnostics;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Update = Telegram.Bot.Types.Update;
using Microsoft.IdentityModel.Tokens;
using Suo.TelegramBotSeparated.Services.RabbitMqService;
using Suo.TelegramBotSeparated.Services.MongoService;

namespace Suo.TelegramBotSeparated.Services.TelegramBotService
{
    public class TelegramBotSeparate
    {
        private ITelegramBotClient _bot;
        private readonly TelegramUserService _teleramService;
        private readonly LogApplicationService _logService;
        private readonly AddToMongoDbService _addToMongoDbService;
        private readonly RabbitService _rabbitMqService;
        private readonly TelegramConfiguration _telegramConfig;
        private readonly IServiceProvider _serviceProvider;
        public static string key = "8asdq728das412zxcq14asd";
        public Dictionary<string, string> Mesages { get; set; } = new Dictionary<string, string>()
    {
        { "FullNumber", "📞 Укажите полный номер телефона" },
        { "Registration", "/registration" },
        { "Imposter", "\U0000274c  Такого жильца нет в общежитии, убедитесь что ввели номер телефона верно" },
        { "Duplicate", "❌  Данный номер уже зарегестрирован в сети, убедитесь что ввели номер телефона верно" },
        { "SendLinc", "Пройдите по ссылке: ⬇⬇⬇" },
        { "EndLinc", "Для завершения регистрации" },
    };
        private string Usernumber { get; set; }


        public TelegramBotSeparate(TelegramConfiguration telegramConfig,
            TelegramUserService teleramService,
            LogApplicationService logService,
            AddToMongoDbService addToMongoDbService,
            RabbitService rabbitMqService
            )
        {
            _bot = new TelegramBotClient(telegramConfig.TelegrammString);
            _teleramService = teleramService;
            _logService = logService;
            _addToMongoDbService = addToMongoDbService;
            _rabbitMqService = rabbitMqService;
            _telegramConfig = telegramConfig;
        }


        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            try
            {
                await _addToMongoDbService.AddFromMessageToMongoDbAsync(update);
                if (update.Type == UpdateType.Message)
                {
                    var passForExist = false;
                    int userId = -1;
                    var message = update.Message;

                    if (message.Text.ToLower() == Mesages["Registration"])
                    {
                        var messageString = Mesages["FullNumber"];
                        await SendMesageWhithReply(botClient, update, messageString);
                    }

                    if (message.ReplyToMessage != null && message.ReplyToMessage.Text.Contains(Mesages["FullNumber"]))
                    {
                        Usernumber = message.Text;
                        if (Usernumber.Length >= 11)
                        {
                            Usernumber = Usernumber.Replace(" ", "");
                            Usernumber = Usernumber.Replace("-", "");
                            Usernumber = Usernumber.Replace("(", "");
                            Usernumber = Usernumber.Replace(")", "");
                            Usernumber = Usernumber.Replace("+", "");
                            if (Usernumber.Length >= 11)
                            {
                                Usernumber = Usernumber.Substring(Usernumber.Length - 11);
                            }
                        }

                        passForExist = _teleramService.CheckExist(Usernumber);

                        if (passForExist == true)
                        {
                            userId = _teleramService.CheckDuble(Usernumber);
                        }

                    }
                    if (passForExist == true && userId != -1)
                    {
                        var token = new Token().GenerateToken(userId);
                        token = Token.Encrypt(token, key);
                        var link = $"{_telegramConfig.IdentityServer}/register/parametertoken/create/{token}";

                        var messageString = $"{Mesages["SendLinc"]}\n{link}\n{Mesages["EndLinc"]}";
                        await SendMesage(botClient, update, messageString);

                        var TeleUsers = _teleramService.Get();
                        TelegramUserVieweModel telUser = new TelegramUserVieweModel()
                        {
                            TelegramUserFirstName = message.From.FirstName,
                            TelegramUserId = message.From.Id.ToString(),
                            UserId = userId
                        };
                        if (!message.From.LastName.IsNullOrEmpty())
                        {
                            telUser.TelegramUserLastName = message.From.LastName;
                        }
                        if (TeleUsers.Contains(telUser))
                        {
                            token = null;
                        }
                        else
                        {
                            _teleramService.Create(telUser);
                        }
                    }
                    else if (userId == -1 && passForExist == true)
                    {
                        var messageString = $"{Mesages["Duplicate"]}\n{Mesages["FullNumber"]}";
                        await SendMesageWhithReply(botClient, update, messageString);
                    }
                    else if (passForExist == false && message.ReplyToMessage != null)
                    {
                        var messageString = $"{Mesages["Imposter"]}\n{Mesages["FullNumber"]}";
                        await SendMesageWhithReply(botClient, update, messageString);
                    }
                    if (message.ReplyToMessage == null && message.Text.ToLower() != Mesages["Registration"])
                    {
                        var messageString = $"Хотите выполнить {Mesages["Registration"]}";
                        await SendMesage(botClient, update, messageString);
                    }
                }
            }
            catch (Exception ex)
            {
                _logService.Create(ex.Message, ex.StackTrace, DateTime.Now);
            }
        }
        public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
        }

        public async Task Start()
        {
            {
                Console.WriteLine("Запущен бот" + _bot.GetMeAsync().Result.FirstName);

                var cts = new CancellationTokenSource();
                var cancellationToken = cts.Token;
                var receiverOptions = new ReceiverOptions
                {
                    AllowedUpdates = { },
                };

                _bot.StartReceiving(
                    HandleUpdateAsync,
                    HandleErrorAsync,
                    receiverOptions,
                    cancellationToken
                );

                await _rabbitMqService.CreateConsumer(_bot);//????
            }
        }
        public async Task SendMesage(ITelegramBotClient botClient, Update update, string messageString)
        {
            try
            {
                await _addToMongoDbService.AddToMessageToMongoDbAsync(update, messageString);
                await botClient.SendTextMessageAsync(update.Message.Chat, messageString);
            }
            catch (Exception ex)
            {
                _logService.Create(ex.Message, ex.StackTrace, DateTime.Now);
            }
        }
        public async Task SendMesageWhithReply(ITelegramBotClient botClient, Update update, string messageString)
        {
            try
            {
                await _addToMongoDbService.AddToMessageToMongoDbAsync(update, messageString);
                await botClient.SendTextMessageAsync(update.Message.Chat, messageString, replyMarkup: new ForceReplyMarkup { Selective = true });
            }
            catch (Exception ex)
            {
                _logService.Create(ex.Message, ex.StackTrace, DateTime.Now);
            }
        }
        public async Task SendDutyMesage(TelegramUserVieweModel telegramUser)
        {
            try
            {
                if (telegramUser.TelegramUserLastName != null)
                {
                    await _bot.SendTextMessageAsync(telegramUser.TelegramUserId, $"🔔 Уважаемый {telegramUser.TelegramUserFirstName.Trim()} {telegramUser.TelegramUserLastName.Trim()}, завтра у вас дежурство.");
                }
                else
                {
                    await _bot.SendTextMessageAsync(telegramUser.TelegramUserId, $"🔔 Уважаемый {telegramUser.TelegramUserFirstName.Trim()}, завтра у вас дежурство.");
                }
            }
            catch (Exception ex)
            {
                _logService.Create(ex.Message, ex.StackTrace, DateTime.Now);
            }
        }


    }
}
