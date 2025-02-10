using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using EversayApi.Data;
using User_lib;

namespace EversayApi.Controllers
{
    public class UserController : Controller
    {
        private readonly IMongoCollection<User>? _users;
        public UserController(MongoDbService mongoDbServiceUser)
        {
            _users = mongoDbServiceUser.Database?.GetCollection<User>("user");
        }

        [HttpGet]
        public async Task<IEnumerable<User>> GetAllUsers()
        {
            return await _users.Find(FilterDefinition<User>.Empty).ToListAsync();
        }

        [HttpGet("{UserName}")]
        public async Task<ActionResult<User>> GetUserByName(string uName)
        {
            var filter = Builders<User>.Filter.Eq("UserName", uName);
            var foundUser = _users.Find(filter).FirstOrDefault();
            return foundUser is not null ? Ok(foundUser) : NotFound();
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
                createdUser.ProfilePicture = "";
            }

            return Ok(createdUser);
        }
    }
}
