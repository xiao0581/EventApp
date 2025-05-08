using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Text.Json.Serialization;

namespace Modules.EventImage
{
    public class EventImage
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? ImageId { get; set; }

        [BsonElement("event_id"), BsonRepresentation(BsonType.String)]
        public string EventId { get; set; }

        [BsonElement("user_id"), BsonRepresentation(BsonType.String)]
        public string UserId { get; set; }

        [BsonElement("image_url"), BsonRepresentation(BsonType.String)]
        public string ImageUrl { get; set; }

        [BsonElement("image_description"), BsonRepresentation(BsonType.String)]
        public string ImageDescription { get; set; }

        [BsonElement("created_at"), BsonRepresentation(BsonType.DateTime)]
        public DateTime CreatedAt { get; set; }
      


    }
}
