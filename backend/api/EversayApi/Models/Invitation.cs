using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace EversayApi.Models
{
    public class Invitation
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string InviteCode { get; set; }
        public string EventId { get; set; }
        public string? AcceptedUserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime AcceptedAt { get; set; }
        public string? DeclinedUserId { get; set; }
        public DateTime? DeclinedAt { get; set; }

    }
}
