using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Event_lib
{
    public class Event
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? eventId { get; set; }

        [BsonElement("event_title"), BsonRepresentation(BsonType.String)]
        public required string EventTitle { get; set; }

        [BsonElement("event_description"), BsonRepresentation(BsonType.String)]
        public required string EventDescription { get; set; }

        [BsonElement("event_date"), BsonRepresentation(BsonType.DateTime)]
        public DateTime EventDate { get; set; }

        [BsonElement("created_at"), BsonRepresentation(BsonType.DateTime)]
        public DateTime CreatedAt { get; set; }

        [BsonElement("expired_at"), BsonRepresentation(BsonType.DateTime)]
        public DateTime ExpiredAt { get; set; }

        public string? EventImage { get; set; }
    }
}
