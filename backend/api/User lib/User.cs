using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace User_lib
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string? userId { get; set; }

        [BsonElement("user_name"), BsonRepresentation(BsonType.String)]
        public required string UserName { get; set; }

        [BsonElement("email"), BsonRepresentation(BsonType.String)]
        public string? Email { get; set; } = null;

        [BsonElement("is_verified"), BsonRepresentation(BsonType.Boolean)]
        public bool IsVerified => !string.IsNullOrEmpty(Email);

        [BsonElement("user_role"), BsonRepresentation(BsonType.String)]
        public UserType UserRole { get; set; } = UserType.Guest;

        [BsonElement("profile_picture"), BsonRepresentation(BsonType.String)]
        public string? ProfilePicture { get; set; }
        public required string PasswordHash { get; set; }
    }
    public enum UserType
    {
        Guest,
        Organizer
    }
}
