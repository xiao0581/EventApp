using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace GuestList_lib
{
    public class GuestList
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? GuestsId { get; set; }

        [BsonElement("event_id"), BsonRepresentation(BsonType.String)] //connect to Event_lib this is to be the event_id, connecting a guest list to an event
        public  string EventId { get; set; }

        [BsonElement("User_id"), BsonRepresentation(BsonType.String)] //connect to User_lib, this is to be the user_id, collecting a list of guests/users
        public  string UserId { get; set; }

        [BsonElement("is_attending"), BsonRepresentation(BsonType.Boolean)]
        public  bool IsAttending { get; set; }

        [BsonElement("guestlist_name"), BsonRepresentation(BsonType.String)]
        public  string GuestListName { get; set; }

        [BsonElement("attendees"), BsonRepresentation(BsonType.Int32)]
        public  int Attendees { get; set; }

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
