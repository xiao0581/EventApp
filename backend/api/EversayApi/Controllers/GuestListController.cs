using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Bson;
using EversayApi.Data;
using GuestList_lib;
using Microsoft.AspNetCore.Authorization;
using Modules.Auth;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace EversayApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GuestListController : ControllerBase
    {
        private readonly IMongoCollection<GuestList>? _guestLists;
        private readonly IMongoCollection<Applicationuser>? _userManager;
        public GuestListController(MongoDbService mongoDbService, UserManager<IdentityUser> userManager)
        {
            _guestLists = mongoDbService.Database?.GetCollection<GuestList>("guestList");
            _userManager = (IMongoCollection<Applicationuser>?)userManager;
        }

        [HttpGet]
        public async Task<IEnumerable<GuestList>> GetAllGuestLists()
        {
            return await _guestLists.Find(FilterDefinition<GuestList>.Empty).ToListAsync();
        }

        [HttpGet("{id}")]
        public ActionResult<GuestList?> GetGuestListById(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId))
            {
                return BadRequest("Invalid ID format");
            }
            var filter = Builders<GuestList>.Filter.Eq("guestListId", id);
            var foundGuestList = _guestLists.Find(filter).FirstOrDefault();
            return foundGuestList is not null ? Ok(foundGuestList) : NotFound();
        }

        [HttpGet("search/{title}")]
        public async Task<IEnumerable<GuestList>> GetGuestListByTitle(string title)
        {
            var filter = Builders<GuestList>.Filter.Regex("guestList_title", new BsonRegularExpression(title, "i"));
            return await _guestLists.Find(filter).ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult> CreateGuestList(GuestList createdGuestList, IFormFile guestListCover)
        {
            if (guestListCover != null)
            {
                MemoryStream memoryStream = new MemoryStream();
                guestListCover.OpenReadStream().CopyTo(memoryStream);
                createdGuestList.GuestListImage = Convert.ToBase64String(memoryStream.ToArray());
            }
            else
            {
                createdGuestList.GuestListImage = "";
            }
            createdGuestList.CreatedAt = DateTime.UtcNow;
            await _guestLists.InsertOneAsync(createdGuestList);
            return Ok(createdGuestList);
        }

        [HttpPut("{id}/add-user")] //in the future add Authorize to Organizer of event
        public async Task<ActionResult> AddUserToGuestList(string id, [FromBody] string userIdToAdd)
        {
            var filter = Builders<GuestList>.Filter.Eq("guestListId", id);
            var guestList = _guestLists.Find(filter).FirstOrDefault();
            if (guestList is null)
            {
                return NotFound("Guest list not found");
            }

            if (guestList.UserIds.Contains(userIdToAdd))
            {
                return BadRequest("User already in guest list");
            }

            guestList.UserIds.Add(userIdToAdd);
            var update = Builders<GuestList>.Update.Set("User_ids", guestList.UserIds);

            await _guestLists.UpdateOneAsync(filter, update);
            return Ok(guestList);
        }
    }
}
