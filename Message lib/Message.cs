using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Message
{
    public class Message
    {
        [BsonElement("msg_id"), BsonRepresentation(BsonType.ObjectId)]
        public required string MessageId { get; set; }

        [BsonElement("msg_text"), BsonRepresentation(BsonType.String)]
        public required string MessageText { get; set; }

        [BsonElement("sent_time"), BsonRepresentation(BsonType.DateTime)]
        public required DateTime MsgSentTime { get; set; }
    }
}
