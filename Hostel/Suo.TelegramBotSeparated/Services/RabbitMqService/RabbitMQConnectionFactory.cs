using RabbitMQ.Client;

namespace Suo.TelegramBotSeparated.Services.RabbitMqService
{
    public class RabbitMQConnectionFactory
    {
        public RabbitMQConnectionFactory(TelegramConfiguration telegramConfiguration)
        {
            TelegramConfiguration = telegramConfiguration;
        }
        public TelegramConfiguration TelegramConfiguration { get; }
        public ConnectionFactory CreateConnectionFactory()
        {
            ConnectionFactory ConnectionFactory = new ConnectionFactory { HostName = TelegramConfiguration.RabbitMQHostName, UserName = TelegramConfiguration.RabbitMQUserName, Password = TelegramConfiguration.RabbitMQPassword };
            return ConnectionFactory;
        }
    }
}
