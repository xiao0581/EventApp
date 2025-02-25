using EversayApi.Data;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using User_lib;

namespace EversayApi.Dtos
{
    public class UserService
    {
        private readonly IMongoCollection<User>? _users;
        private readonly IPasswordHasher<User> _passwordHasher;

        public UserService(MongoDbService dbService, IPasswordHasher<User> passwordHasher)
        {
            _users = dbService.Database.GetCollection<User>("users");
            _passwordHasher = passwordHasher;
        }

        public UserService(MongoDbService dbService)
        {
            _users = dbService.Database.GetCollection<User>("users");
        }

        public async Task<User?> RegisterUser(User user, RegisterRequest registerRequest)
        {
            user.UserName = registerRequest.Name;
            user.Email = registerRequest.Email;
            user.UserRole = UserType.Guest;
            user.ProfilePicture = "https://eversay.dk/wp-content/uploads/2024/12/Frame-2694-2.png";
            user.PasswordHash = _passwordHasher.HashPassword(user, registerRequest.Password);

            await _users.InsertOneAsync(user);
            return user;
        }
    }
}
