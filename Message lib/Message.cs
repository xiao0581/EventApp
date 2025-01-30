using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Message_lib
{
    public class Message
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public required string MessageId { get; set; }

        [BsonElement("msg_text"), BsonRepresentation(BsonType.String)]
        public required string MessageText { get; set; }

        [BsonElement("sent_time"), BsonRepresentation(BsonType.DateTime)]
        public required DateTime MsgSentTime { get; set; }

        [BsonElement("sender_id"), BsonRepresentation(BsonType.ObjectId)]
        public required string SenderId { get; set; }

        [BsonElement("receiver_id"), BsonRepresentation(BsonType.ObjectId)]
        public required string ReceiverId { get; set; }

        [BsonElement("is_read"), BsonRepresentation(BsonType.Boolean)]
        public required bool IsRead { get; set; }

        [BsonElement("attachment"), BsonRepresentation(BsonType.String)]
        public string? MessageAttachment { get; set; }
    }
}
