using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using EversayApi.Data;
using User_lib;
using EversayApi.Dtos;
using Microsoft.AspNetCore.Authorization;

namespace EversayApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : Controller
    {
        private readonly IMongoCollection<User>? _users;
        public UserController(MongoDbService mongoDbServiceUser)
        {
            _users = mongoDbServiceUser.Database?.GetCollection<User>("users");
        }

        [HttpGet]
        public async Task<IEnumerable<User>> GetAllUsers()
        {
            return await _users.Find(FilterDefinition<User>.Empty).ToListAsync();
        }

        [HttpGet("id/{userId}")]
        public async Task<ActionResult<PublicUserDto>> GetUserById(string userId)
        {
            var filter = Builders<User>.Filter.Eq("_id", userId);
            var foundUser = await _users.Find(filter).FirstOrDefaultAsync();

            if (foundUser is null)
                return NotFound($"User with ID {userId} not found.");

         
            var publicUser = new PublicUserDto
            {
                userId = foundUser.userId!,
                UserName = foundUser.UserName,
                Email = foundUser.Email,
                ProfilePicture = foundUser.ProfilePicture,
                UserRole = foundUser.UserRole
            };

            return Ok(publicUser);
        }

        [HttpPost]
        public async Task<ActionResult> CreateUser(User createdUser, IFormFile profilePic)
        {
            if (profilePic != null)
            {
                MemoryStream memoryStream = new MemoryStream();
                profilePic.OpenReadStream().CopyTo(memoryStream);
                createdUser.ProfilePicture = Convert.ToBase64String(memoryStream.ToArray());
            }
            else
            {
                createdUser.ProfilePicture = "https://eversaydevne.blob.core.windows.net/eversaydev/unicorn_avatar.png";
            }

            return Ok(createdUser);
        }
    }
}
