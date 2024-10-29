using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Suo.Client.Models;
using System.Text;
using System.Text.Json;

namespace Suo.Client.Data.RabbitMqService
{
    public class RabbitService
    {
        private ConnectionFactory _factory;
        private IConnection _connection;
        private IModel _channel;
        public RabbitService()
        {
            _factory = new ConnectionFactory { HostName = "localhost", UserName = "ruser", Password = "wlad1051" };
            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        public async Task CreateQueue()
        {
            _channel.QueueDeclare(queue: "suoMesages",
               durable: true,
               exclusive: false,
               autoDelete: false,
               arguments: null);
        }

        public async Task SendMessageToTgBot(MessageModelForTg messageModel)
        {
            CreateQueue();//стоит ли вызывать каждый раз в случае если очередь одна?
            string jsonModel = JsonSerializer.Serialize(messageModel);
            _channel.BasicPublish(exchange: string.Empty,
                routingKey: "suoMesages",
                body: ConvertMessage(jsonModel));
        }

        private byte[] ConvertMessage(string jsonModel)
        {
            return Encoding.UTF8.GetBytes(jsonModel);
        }
    }
}
