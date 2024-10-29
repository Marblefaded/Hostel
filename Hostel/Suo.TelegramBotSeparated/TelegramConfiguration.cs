using RabbitMQ.Client;

namespace Suo.TelegramBotSeparated
{
    public class TelegramConfiguration
    {
        public string IdentityServer { get; set; }
        public string TelegrammString { get; set; }
        public string ConnectionString { get; set; }
        public string MongoDbString { get; set; }
        public string RabbitMQHostName { get; set; }
        public string RabbitMQUserName { get; set; }
        public string RabbitMQPassword { get; set; }

       // public ConnectionFactory ConnectionFactory = new ConnectionFactory { HostName = "localhost", UserName = "ruser", Password = "wlad1051" };
    }
}
