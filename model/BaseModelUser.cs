using MongoDB.Bson.Serialization.Attributes;

namespace Twillink.Shared.Models
{
    public class BaseModelUser
    {
        [BsonElement("CreatedAt")]
        public DateTime CreatedAt { get; set; }
        [BsonElement("UpdatedAt")]
        public DateTime? UpdatedAt { get; set; }
        [BsonElement("IsActive")]
        public bool? IsActive { get; set; }
        [BsonElement("IsVerification")]
        public bool? IsVerification {get; set;}
        [BsonElement("UserId")]
        public bool? UserId {get; set;}
        [BsonElement("TwillinkId")]
        public bool? TwillinkId {get; set;}

    }
}