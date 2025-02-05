using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Bson;
using EversayApi.Data;
using Event_lib;

namespace EversayApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IMongoCollection<Event>? _events;
        public EventController(MongoDbService mongoDbService)
        {
            _events = mongoDbService.Database?.GetCollection<Event>("event");
        }

        [HttpGet]
        public async Task<IEnumerable<Event>> GetAllEvents()
        {
            return await _events.Find(FilterDefinition<Event>.Empty).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Event?>> GetEventById(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId))
            {
                return BadRequest("Invalid ID format");
            }

            var filter = Builders<Event>.Filter.Eq("eventId", id);
            var foundEvent = _events.Find(filter).FirstOrDefault();
            return foundEvent is not null ? Ok(foundEvent) : NotFound();
        }

        [HttpGet("search/{title}")]
        public async Task<IEnumerable<Event>> GetEventByTitle(string title)
        {
            var filter = Builders<Event>.Filter.Regex("event_title", new BsonRegularExpression(title, "i"));
            return await _events.Find(filter).ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult> CreateEvent(Event createdEvent, IFormFile eventCover)
        {
            if (eventCover != null)
            {
                MemoryStream memoryStream = new MemoryStream();
                eventCover.OpenReadStream().CopyTo(memoryStream);
                createdEvent.EventImage = Convert.ToBase64String(memoryStream.ToArray());
            }
            else
            { //comment
                createdEvent.EventImage = "";
            }

            createdEvent.CreatedAt = DateTime.UtcNow;
            createdEvent.ExpiredAt = createdEvent.EventDate.AddDays(100);
            await _events.InsertOneAsync(createdEvent);
            return CreatedAtAction(nameof(GetEventById), new { id = createdEvent.eventId }, createdEvent);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateEvent(Event updatedEvent)
        {
            var filter = Builders<Event>.Filter.Eq("eventId", updatedEvent.eventId);
            await _events.ReplaceOneAsync(filter, updatedEvent);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteEvent(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId))
            {
                return BadRequest("Invalid ID format");
            }

            var filter = Builders<Event>.Filter.Eq("eventId", id);
            var result = await _events.DeleteOneAsync(filter);

            if (result.DeletedCount > 0)
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }
    }
}
