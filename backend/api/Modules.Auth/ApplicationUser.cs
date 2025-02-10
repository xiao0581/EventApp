using AspNetCore.Identity.MongoDbCore.Models;
using MongoDB.Driver;
using MongoDbGenericRepository.Attributes;

namespace Modules.Auth
{
    [CollectionName("users")]
    public class Applicationuser : MongoIdentityUser<Guid>
    {
        public string Name { get; set; } = string.Empty;
    }
}

