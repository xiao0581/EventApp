using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Bson;
using EversayApi.Data;
using Event_lib;
using User_lib;

namespace EversayApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventImageController : ControllerBase
    {
        private readonly IMongoCollection<Event>? _events;
        private readonly IMongoCollection<User>? _users;

        public EventImageController(MongoDbService mongoDbService)
        {
            _events = mongoDbService.Database?.GetCollection<Event>("events");
            _users = mongoDbService.Database?.GetCollection<User>("users");
        }

        [HttpGet("{eventId}/coverimage")]
        public async Task<ActionResult<IEnumerable<string>>> GetEventImages(string eventId)
        {
            if (!ObjectId.TryParse(eventId, out var objectId))
            {
                return BadRequest("Invalid ID format");
            }
            var filter = Builders<Event>.Filter.Eq("eventId", eventId);
            var foundEvent = await _events.Find(filter).FirstOrDefaultAsync();
            if (foundEvent == null)
            {
                return NotFound("Event not found");
            }
            var images = new List<string>();
            if (!string.IsNullOrEmpty(foundEvent.EventImage))
            {
                images.Add(foundEvent.EventImage);
            }
            return Ok(images);
        }
    }
}
