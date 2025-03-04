using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Bson;
using EversayApi.Data;
using Event_lib;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Modules.Auth;

namespace EversayApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EventController : ControllerBase
    {
        private readonly IMongoCollection<Event>? _events;
        public EventController(MongoDbService mongoDbService)
        {
            _events = mongoDbService.Database?.GetCollection<Event>("event");
        }

        public static string GenerateInvitationLink(string eventId)
        {
            return $"https://eversay.com/event/{eventId}"; //change this to the actual domain
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

        [HttpGet("orderbydate")] //might want to change this to a different route
        public async Task<IEnumerable<Event>> OrderByDateAsc(DateTime date)
        {
            var filter = Builders<Event>.Filter.Gt("event_date", date);
            return await _events.Find(filter).ToListAsync();
        }

        [HttpGet("searchbyuser/{Name}")]
        public async Task<IEnumerable<Event>> GetEventByUser(string Name)
        {
            var filter = Builders<Event>.Filter.Eq("name", Name);
            return await _events.Find(filter).ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult> CreateEvent(Event createdEvent)
        {
            var name = User.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest("User not found");
            }

            createdEvent.Name = name; //eventually change this to the user's ID

            if (string.IsNullOrEmpty(createdEvent.EventImage))
            {
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
