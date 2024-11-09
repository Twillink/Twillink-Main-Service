using MongoDB.Bson.Serialization.Attributes;
namespace Twillink.Shared.Models
{
public class AddUrlTwillink : BaseModel
    {
        [BsonId]
        public string? Title {get; set;}

        [BsonElement("UserId")]
        public string? UserId {get; set;}
    }
}