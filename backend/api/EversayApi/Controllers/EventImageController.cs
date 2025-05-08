using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Bson;
using EversayApi.Data;
using Event_lib;
using User_lib;
using Modules.EventImage;
using Event_lib.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace EversayApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EventImageController : ControllerBase
    {
        private readonly EventImageRepository _repository;

        public EventImageController(MongoDbService mongoDbService)
        {
            var database = mongoDbService.Database!;
            _repository = new EventImageRepository(database);
        }


        [HttpGet("getAllImage")]

        public async Task<IActionResult> GetAll()
        {
            var images = await _repository.GetAllImagesAsync();
            return Ok(images);
        }


        [HttpGet("getImageByevent/{eventId}")]
        public async Task<IActionResult> GetByEventId(string eventId)
        {
            var images = await _repository.GetImagesByEventId(eventId);
            if (images == null || images.Count == 0)
                return NotFound("No images found for this event.");
            return Ok(images);
        }

       
        [HttpGet("getImageByuser/{userId}")]
        public async Task<IActionResult> GetByUserId(string userId)
        {
            var images = await _repository.GetImagesByUserId(userId);
            
            return Ok(images ?? new List<EventImage>());
        }

       
        [HttpPost("byevent/{eventId}")]
        public async Task<IActionResult> AddImageByEventId(string eventId, [FromBody] EventImage image)
        {
            image.EventId = eventId;
            image.CreatedAt = DateTime.UtcNow;
            var success = await _repository.AddImageByEventIdAsync(image);
            if (!success) return BadRequest("EventId is missing.");
            return Ok("Image added by eventId.");
        }

        
        [HttpPost("byuser/{userId}")]
        public async Task<IActionResult> AddImageByUserId(string userId, [FromBody] EventImage image)
        {
            image.UserId = userId;
            image.CreatedAt = DateTime.UtcNow;
            var success = await _repository.AddImageByUserIdAsync(image);
            if (!success) return BadRequest("UserId is missing.");
            return Ok("Image added by userId.");
        }

        //[HttpGet("{eventId}/coverimage")]
        //public async Task<ActionResult<IEnumerable<string>>> GetEventImage(string eventId)
        //{
        //    if (!ObjectId.TryParse(eventId, out var objectId))
        //    {
        //        return BadRequest("Invalid ID format");
        //    }
        //    var filter = Builders<Event>.Filter.Eq("eventId", eventId);
        //    var foundEvent = await _events.Find(filter).FirstOrDefaultAsync();
        //    if (foundEvent == null)
        //    {
        //        return NotFound("Event not found");
        //    }
        //    var images = new List<string>();
        //    if (!string.IsNullOrEmpty(foundEvent.EventImage))
        //    {
        //        images.Add(foundEvent.EventImage);
        //    }
        //    return Ok(images);
        //}

        //[HttpPost("{eventId}/images")]
        //public async Task<IActionResult> AddImages(string eventId, List<IFormFile> images)
        //{
        //    if (!ObjectId.TryParse(eventId, out var objectId))
        //    {
        //        return BadRequest("Invalid ID format");
        //    }

        //    var filter = Builders<Event>.Filter.Eq("eventId", eventId);
        //    var foundEvent = await _events.Find(filter).FirstOrDefaultAsync();

        //    if (foundEvent == null)
        //    {
        //        return NotFound("Event not found");
        //    }

        //    if (images == null || images.Count == 0)
        //    {
        //        return BadRequest("No images provided");
        //    }

        //    if (foundEvent.EventImages == null)
        //    {
        //        foundEvent.EventImages = new List<string>();
        //    }

        //    foreach (var image in images)
        //    {
        //        using (var memoryStream = new MemoryStream())
        //        {
        //            await image.CopyToAsync(memoryStream);
        //            var base64Image = Convert.ToBase64String(memoryStream.ToArray());
        //            foundEvent.EventImages.Add(base64Image);
        //        }
        //    }

        //    await _events.ReplaceOneAsync(filter, foundEvent);
        //    return Ok(new {message = "Images added successfully", images = foundEvent.EventImages });
        //}
    }
}
