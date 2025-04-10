using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Bson;
using EversayApi.Data;
using Event_lib;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using User_lib;
using GuestList_lib;

namespace EversayApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EventController : ControllerBase
    {
        private readonly IMongoCollection<Event>? _events;
        private readonly IMongoCollection<User>? _users;
        private readonly IMongoCollection<GuestList>? _guestList;

        public EventController(MongoDbService mongoDbService)
        {
            _events = mongoDbService.Database?.GetCollection<Event>("event");
            _guestList = mongoDbService.Database?.GetCollection<GuestList>("guestlists");
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

        [HttpGet("searchbyuser")] //searches the currently logged in user's events
        public async Task<IEnumerable<Event>> GetEventByUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Enumerable.Empty<Event>();
            }
            var filter = Builders<Event>.Filter.Eq("created_by", userId);
            return await _events.Find(filter).ToListAsync();
        }

        [HttpGet("searchbyuser/{userName}")] //searches the events of a specific user
        public async Task<IEnumerable<Event>> GetEventByUserName(string userName)
        {
            var userFilter = Builders<User>.Filter.Eq("userName", userName);
            var user = await _users.Find(userFilter).FirstOrDefaultAsync();

            if (user == null)
            {
                return Enumerable.Empty<Event>();
            }

            var eventFilter = Builders<Event>.Filter.Eq("created_by", user.userId);
            return await _events.Find(eventFilter).ToListAsync();
        }

        [HttpGet("searchbyid/{userId}")]
        public async Task<IEnumerable<Event>> GetEventByUserId(string userId)
        {
            var filter = Builders<Event>.Filter.Eq("created_by", userId);
            return await _events.Find(filter).ToListAsync();
        }


        [HttpGet("byguest/{userId}")]
        public async Task<ActionResult<IEnumerable<Event>>> GetEventsByGuestUserId(string userId)
        {
            try
            {
                var guestListFilter = Builders<GuestList>.Filter.Eq("user_id", userId);
                var guestEntries = await _guestList.Find(guestListFilter).ToListAsync();

                if (guestEntries == null || guestEntries.Count == 0)
                {
                    return NotFound(new { message = "No events found for this user." });
                }

                var eventIds = new List<ObjectId>();
                foreach (var entry in guestEntries)
                {
                    if (ObjectId.TryParse(entry.EventId, out ObjectId objectId))
                    {
                        eventIds.Add(objectId);
                    }
                    else
                    {
                        Console.WriteLine($"Invalid ObjectId format: {entry.EventId}");
                    }
                }

                if (eventIds.Count == 0)
                {
                    return BadRequest(new { message = "No valid event IDs found." });
                }

                var eventFilter = Builders<Event>.Filter.In("_id", eventIds);
                var events = await _events.Find(eventFilter).ToListAsync();

                return Ok(events);
            }
            catch (MongoException ex)
            {             
                Console.WriteLine($"MongoDB error: {ex.Message}");
                return StatusCode(500, new { message = "Database error occurred. Please try again later." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }


        [HttpPost]
        public async Task<ActionResult> CreateEvent([FromBody] Event createdEvent)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User not found");
            }

            createdEvent.CreatedBy = userId;

            if (string.IsNullOrEmpty(createdEvent.EventImage))
            {
                createdEvent.EventImage = "";
            }
            if (string.IsNullOrEmpty(createdEvent.CreatedBy))
            {
                createdEvent.CreatedBy = userId;
            }

            createdEvent.CreatedAt = DateTime.UtcNow;
            createdEvent.ExpiredAt = createdEvent.EventDate.AddDays(100);
            await _events.InsertOneAsync(createdEvent);
            var insertedEventId = createdEvent.eventId;

            var guestEntry = new GuestList
            {
                EventId = insertedEventId.ToString(),
                UserId = userId.ToString(),
                IsAttending = true,
                GuestListName = createdEvent.EventTitle,
                Attendees = 1,
                GuestListImage = "",
                CreatedAt = DateTime.UtcNow
            };

            await _guestList.InsertOneAsync(guestEntry);
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
