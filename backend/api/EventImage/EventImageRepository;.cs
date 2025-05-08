using MongoDB.Driver;
using Modules.EventImage;

namespace Event_lib.Repositories
{
    public class EventImageRepository
    {
        private readonly IMongoCollection<EventImage> _eventImages;

        public EventImageRepository(IMongoDatabase database)
        {
            _eventImages = database.GetCollection<EventImage>("eventimages");
        }

        public async Task<List<EventImage>> GetAllImagesAsync()
        {
            return await _eventImages.Find(_ => true).ToListAsync();
        }
        public async Task<List<EventImage>> GetImagesByEventId(string eventId)
        {
            var filter = Builders<EventImage>.Filter.Eq(img => img.EventId, eventId);
            return await _eventImages.Find(filter).ToListAsync();
        }

        public async Task<List<EventImage>> GetImagesByUserId(string userId)
        {
            var filter = Builders<EventImage>.Filter.Eq(img => img.UserId, userId);
            return await _eventImages.Find(filter).ToListAsync();
        }

        public async Task<bool> AddImageByEventIdAsync(EventImage image)
        {
            if (string.IsNullOrEmpty(image.EventId))
                return false;

            image.CreatedAt = DateTime.UtcNow;
            await _eventImages.InsertOneAsync(image);
            return true;
        }

        public async Task<bool> AddImageByUserIdAsync(EventImage image)
        {
            if (string.IsNullOrEmpty(image.UserId))
                return false;

            image.CreatedAt = DateTime.UtcNow;
            await _eventImages.InsertOneAsync(image);
            return true;
        }
    }
}
