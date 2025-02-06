using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GuestList_lib;

public class GuestList
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? guestsId { get; set; }

    [BsonElement("event_id"), BsonRepresentation(BsonType.ObjectId)] //connect to Event_lib this is to be the event_id, connecting a guest list to an event
    public required string eventId { get; set; }

    [BsonElement("guest_id"), BsonRepresentation(BsonType.ObjectId)] //connect to User_lib, this is to be the user_id, collecting a list of guests/users
    public required string guestId { get; set; }

    [BsonElement("is_attending"), BsonRepresentation(BsonType.Boolean)]
    public required bool IsAttending { get; set; }

    [BsonElement("guestlist_name"), BsonRepresentation(BsonType.String)]
    public required string GuestListName { get; set; }

    [BsonElement("attendees"), BsonRepresentation(BsonType.Int32)]
    public required int Attendees { get; set; }

    [BsonElement("guestlist_image"), BsonRepresentation(BsonType.String)]
    public string? GuestListImage { get; set; }

    [BsonElement("created_at"), BsonRepresentation(BsonType.DateTime)]
    public DateTime CreatedAt { get; set; }
}
