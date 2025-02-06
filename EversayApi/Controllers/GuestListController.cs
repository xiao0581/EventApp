using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Bson;
using EversayApi.Data;
using Modules.GuestListLib;
namespace EversayApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GuestListController : ControllerBase
    {
        private readonly IMongoCollection<GuestList>? _guestLists;
        public GuestListController(MongoDbService mongoDbService)
        {
            _guestLists = mongoDbService.Database?.GetCollection<GuestList>("guestList");
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
    }
}
