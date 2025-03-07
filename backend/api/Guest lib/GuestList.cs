using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace GuestList_lib
{
    public class GuestList
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? GuestsId { get; set; }

        [BsonElement("event_id"), BsonRepresentation(BsonType.String)]
        public string EventId { get; set; }

        [BsonElement("user_id"), BsonRepresentation(BsonType.String)]
        public string UserId { get; set; }

        [BsonElement("User_ids"), BsonRepresentation(BsonType.String)] //list of user ids that are attending the event
        public List<string> UserIds { get; set; } = new List<string>();

        [BsonElement("is_attending"), BsonRepresentation(BsonType.Boolean)]
        public  bool IsAttending { get; set; }

        [BsonElement("guestlist_name"), BsonRepresentation(BsonType.String)]
        public string GuestListName { get; set; }

        [BsonElement("attendees"), BsonRepresentation(BsonType.Int32)]
        public int Attendees { get; set; }

        [BsonElement("guestlist_image"), BsonRepresentation(BsonType.String)]
        public string? GuestListImage { get; set; }

        [BsonElement("created_at"), BsonRepresentation(BsonType.DateTime)]
        public DateTime CreatedAt { get; set; }

        public void ValidateGuestListName()
        {
            if (string.IsNullOrEmpty(GuestListName))
            {
                throw new ArgumentException("Guest list name cannot be empty");
            }
        }
    }
}
