using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using EversayApi.Data;
using Message_lib;

namespace EversayApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : Controller
    {
        private const long maxAllowedSize = 10 * 1024 * 1024; //10MB, can be changed
        private readonly IMongoCollection<Message>? _messages;
        public MessageController(MongoDbService mongoDbService)
        {
            _messages = mongoDbService.Database?.GetCollection<Message>("message");
        }

        [HttpGet("api/message/inbox")] //idea here is to have an inbox for each user/group that has sent a message
        public async Task<IEnumerable<Message>> GetInboxMessages(string userId)
        {
            var filter = Builders<Message>.Filter.Eq("receiver_id", userId);
            return await _messages.Find(filter).ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult> SendMessage(Message sentMessage, IFormFile msgAttachment)
        {
            if (msgAttachment != null)
            {
                MemoryStream memoryStream = new MemoryStream();
                msgAttachment.OpenReadStream().CopyTo(memoryStream); //need to change max file size but maxAllowedSize is not working for some reason
                sentMessage.MessageAttachment = Convert.ToBase64String(memoryStream.ToArray());
            }
            else
            {
                sentMessage.MessageAttachment = "";
            }

            if (sentMessage.ReceiverId == null || sentMessage.SenderId == null)
            {
                return BadRequest("ReceiverId and SenderId are required");
            }

            if (sentMessage.MessageText == null && sentMessage.MessageAttachment == null)
            {
                return BadRequest("Message needs content");
            }
            sentMessage.MsgSentTime = DateTime.UtcNow;
            await _messages.InsertOneAsync(sentMessage);
            return Ok(sentMessage);
        }
    }
}
