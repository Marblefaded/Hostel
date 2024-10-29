using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.Extensions.DependencyInjection;
using JWTCoding;
using Microsoft.Data.SqlClient;
using Suo.Autorization.Data.Service;
using MySqlConnector;
using Telegram.Bot.Types.Enums;

namespace Suo.Autorization.TelegramBot
{
    public class TelegramBotNotification
    {
        public IServiceScopeFactory _serviceScopeFactory;
        public TelegramBotNotification(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public IServiceProvider _serviceProvider;
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public static string key = "8asdq728das412zxcq14asd";

        string usernumber;

        ITelegramBotClient bot = new TelegramBotClient(TelegramConfiguration.TelegrammString);

        TelegramService Service;

        Token Token;

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));
            try
            {
                if (update.Type == UpdateType.Message)
                {
                    var passForExist = false;
                    int userId = -1;
                    var message = update.Message;

                    if (message.Text.ToLower() == "/registration")
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, "📞 Укажите полный номер телефона", replyMarkup: new ForceReplyMarkup { Selective = true });
                    }

                    if (message.ReplyToMessage != null && message.ReplyToMessage.Text.Contains("📞 Укажите полный номер телефона"))
                    {
                        usernumber = message.Text;
                        if (usernumber.Length >= 11)
                        {
                            usernumber = usernumber.Replace(" ", "");
                            usernumber = usernumber.Replace("-", "");
                            usernumber = usernumber.Replace("(", "");
                            usernumber = usernumber.Replace(")", "");
                            usernumber = usernumber.Replace("+", "");
                            if (usernumber.Length >= 11)
                            {
                                usernumber = usernumber.Substring(usernumber.Length - 11);
                            }
                        }

                        //using (MySqlConnection connection = new MySqlConnection(TelegramConfiguration.ConnectionString))
                        //{                      
                        passForExist = Service.CheckExist(usernumber);

                        if (passForExist == true)
                        {
                            //using (MySqlConnection connectionForDuble = new MySqlConnection(TelegramConfiguration.ConnectionString))
                            //{
                            //    var queryString = $"SELECT *FROM hostel.User u WHERE EXISTS (SELECT *FROM hostel.AspNetUsers anu WHERE anu.UserId = u.UserId AND u.UserId = '{userId}')";
                            //    MySqlCommand commandForDubleCheck = new MySqlCommand(queryString, connectionForDuble);
                            //    connectionForDuble.Open();
                            //    MySqlDataReader readerForDubleCheck = commandForDubleCheck.ExecuteReader();
                            //    //SqlDataReader readerForDubleCheck = null;
                            //    try
                            //    {
                            //        if (readerForDubleCheck.HasRows)
                            //        {
                            //            passForDuble = false;
                            //        }
                            //        else { passForDuble = true; }
                            //    }
                            //    finally
                            //    {
                            //        // Always call Close when done reading.
                            //        readerForDubleCheck.Close();
                            //    }
                            //}                  
                            userId = Service.CheckDuble(usernumber);
                        }
                        //}
                    }
                    if (passForExist == true && userId != -1)
                    {
                        var cleartoken = Token.GenerateToken(userId);//выдать шифрованный
                        var token = Token.Encrypt(cleartoken,key);
                        var link = $"{TelegramConfiguration.IdentityServer}/register/parametertoken/create/{token}";
                        await botClient.SendTextMessageAsync(message.Chat.Id, $"Пройдите по ссылке: ⬇⬇⬇");
                        await botClient.SendTextMessageAsync(message.Chat.Id, $"{link}");

                        var TeleUsers = Service.Get();
                        TelegramUserVieweModel telUser = new TelegramUserVieweModel();
                        telUser.TelegramUserFirstName = message.From.FirstName;
                        telUser.TelegramUserLastName = message.From.LastName;
                        telUser.TelegramUserId = message.From.Id.ToString();
                        telUser.UserId = userId;
                        if (TeleUsers.Contains(telUser))
                        {
                            token = null;
                        }
                        else
                        {
                            Service.Create(telUser);
                        }
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Для завершения регистрации");
                    }
                    else if (userId == -1 && passForExist == true)
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, "\U0000274c  Данный номер уже зарегестрирован в сети, убедитесь что ввели номер телефона верно");
                        await botClient.SendTextMessageAsync(message.Chat.Id, "📞 Укажите полный номер телефона", replyMarkup: new ForceReplyMarkup { Selective = true });
                    }
                    else if (passForExist == false && message.ReplyToMessage != null)
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, "\U0000274c  Такого жильца нет в общежитии, убедитесь что ввели номер телефона верно");
                        await botClient.SendTextMessageAsync(message.Chat.Id, "📞 Укажите полный номер телефона", replyMarkup: new ForceReplyMarkup { Selective = true });
                    }
                    if (message.ReplyToMessage == null && message.Text.ToLower() != "/registration")
                    {
                        await botClient.SendTextMessageAsync(message.Chat, "Хотите выполнить  /registration");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));

        }

        public async Task Start()
        {
            var scope = _serviceProvider.CreateScope();
            {
                Token = new Token();
                Service = scope.ServiceProvider.GetRequiredService<TelegramService>();

                Console.WriteLine("Запущен бот " + bot.GetMeAsync().Result.FirstName);

                var cts = new CancellationTokenSource();
                var cancellationToken = cts.Token;
                var receiverOptions = new ReceiverOptions
                {
                    AllowedUpdates = { },
                };

                bot.StartReceiving(
                    HandleUpdateAsync,
                    HandleErrorAsync,
                    receiverOptions,
                    cancellationToken
                );
            }
        }
        public async Task SendDutyMesage(string chatId, string firstname, string lastname)
        {
            try
            {
                if (lastname != null)
                {
                    await bot.SendTextMessageAsync(chatId, ($"🔔 Уважаемый {firstname.Trim()} {lastname.Trim()}, завтра у вас дежурство."));
                }
                else
                {
                    await bot.SendTextMessageAsync(chatId, ($"🔔 Уважаемый {firstname.Trim()}, завтра у вас дежурство."));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

            }

        }

        public async Task SendClaimTakeMesage(string chatId)
        {
            try
            {
                await bot.SendTextMessageAsync(chatId, "Ваше заявление успешно подано");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

            }
        }

        public async Task SendClaimApprovedMessage(string chatId)
        {
            try
            {
                await bot.SendTextMessageAsync(chatId, "Ваше заявление успешно принято");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

            }
        }
        public async Task SendClaimNotApprovedMessage(string chatId)
        {
            try
            {
                await bot.SendTextMessageAsync(chatId, "Ваше заявление отклонено");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

            }
        }
        public async Task SendResetMesage(string chatId, int userId)
        {
            try
            {
                var cleartoken = (new Token()).GenerateToken(userId);//выдать шифрованный
                var token = Token.Encrypt(cleartoken, key);
                await bot.SendTextMessageAsync(chatId, "Перейдите по ссылке для восстановления пароля: ⬇⬇⬇");
                var link = $"{TelegramConfiguration.IdentityServer}/reset/resetpassword/reset/{token}";
                await bot.SendTextMessageAsync(chatId, $"{link}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

            }
        }

    }
}

