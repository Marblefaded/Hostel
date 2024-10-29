using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Suo.TelegramBotSeparated.Models
{
    public class TelegramMessageViewModel
    {
        public ObjectId Id { get; set; }
        public string Direction { get; set; }
        public string User { get; set; }
        public string TextMesage {  get; set; }
        public string DateSend {  get; set; }
    }
}
