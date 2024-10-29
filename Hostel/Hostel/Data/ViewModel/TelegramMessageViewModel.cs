using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Suo.Admin.Data.ViewModel
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
