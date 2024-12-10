using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Twillink.Shared.Models
{
    public class Payment : BaseModel
    {
        [BsonId]
        public string? Id { get; set; }

        [BsonElement("TypePayment")]
        public string? TypePayment { get; set; }

        [BsonElement("IdUser")]
        public string? IdUser { get; set; }
        [BsonElement("Photo")]
        public string? Photo { get; set; }
        [BsonElement("NameUser")]
        public string? NameUser { get; set; }

        [BsonElement("IdItem")]
        public string? IdItem { get; set; }

        [BsonElement("Price")]
        public float? Price { get; set; }
    }
}