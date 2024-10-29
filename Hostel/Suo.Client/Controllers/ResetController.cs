using JWTCoding;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using Suo.Client.Data.Services;
using Suo.Client.Extentions;
using Suo.Client.Models;
using System.Text;
using System.Threading.Channels;

namespace Suo.Client.Controllers
{
    public class ResetController : Controller
    {
        private ConnectionFactory _factory;
        private IConnection _connection;
        private IModel _channel;

        public static string key = "8asdq728das412zxcq14asd";

        public UserService userService;

        private readonly AppConfiguration _config;
        public ErrorConfirm errorConfirm = new ErrorConfirm();
        public ResetController(UserService userService, AppConfiguration config) 
        {
            _factory = new ConnectionFactory { HostName = "localhost", UserName = "ruser", Password = "wlad1051" };
            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();
            this.userService = userService;
            _config = config;
        }

        public IActionResult Index()
        {
            CreateQueue();
            errorConfirm.TelegramLink = _config.TelegramLink;
            return View("Index", errorConfirm);
        }

        public async Task SendMessageToTgBot(MessageModelForTg messageModel)
        {
            string jsonModel = System.Text.Json.JsonSerializer.Serialize(messageModel);
            _channel.BasicPublish(exchange: string.Empty,
                routingKey: "suoMesages",
                body: ConvertMessage(jsonModel));
        }
        private byte[] ConvertMessage(string jsonModel)
        {
            return Encoding.UTF8.GetBytes(jsonModel);
        }
        public async Task CreateQueue()
        {
            _channel.QueueDeclare(queue: "suoMesages",
               durable: true,
               exclusive: false,
               autoDelete: false,
               arguments: null);
        }


        [HttpPost]
        public async Task<IActionResult> Reset(string phonenumber)
        {

            if (phonenumber != null)
            {
                if ((await userService.Check(phonenumber)) == null)
                {
                    errorConfirm.Message = "Пользователь не найден, проверьте ваш номер телефона или пройдите регистрацию.";
                    return View("Index", errorConfirm);
                }
                else
                {
                    var users = await userService.Check(phonenumber);
                    var teleusers = userService.UserInfo(users.UserId);
                    var token = (new Token()).GenerateToken(users.UserId);
                    token = Token.Encrypt(token, key);
                    var link = $"Перейдите по ссылке для восстановления пароля: ⬇⬇⬇ \n{AppConfigGlobals.IdentityServer}/reset/resetpassword/reset/{token}";
                    await SendMessageToTgBot(new MessageModelForTg() { TelegrammUserId = int.Parse(teleusers.TelegramUserId), Message = link });
                    return Redirect("https://t.me/SUO1_Bot");
                }
            }
            else
            {
                errorConfirm.Message = "Введите номер телефона.";
                return View("Index", errorConfirm);
            }
        }
    }
}
