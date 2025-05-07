using EversayApi.Data;
using EversayApi.Dtos;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using User_lib;

namespace EversayApi.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User>? _users;
        private readonly IPasswordHasher<User> _passwordHasher;

        public UserService(MongoDbService dbService, IPasswordHasher<User>? passwordHasher = null)
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
            user.ProfilePicture = "https://eversaydevne.blob.core.windows.net/eversaydev/unicorn_avatar.png";
            user.PasswordHash = _passwordHasher.HashPassword(user, registerRequest.Password);

            await _users.InsertOneAsync(user);
            return user;
        }
    }
}
