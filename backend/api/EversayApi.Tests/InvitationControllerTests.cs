using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using EversayApi.Controllers;
using EversayApi.Data;

namespace EversayApi.Tests
{
    public class InvitationControllerTests
    {
        [Fact]
        public async Task GenerateInvite_EmptyEventId_ReturnsBadRequest()
        {
            // Arrange

            var settings = new Dictionary<string, string>
            {
                { "ConnectionStrings:DbConnection", "mongodb://localhost/monodemo" }
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(settings!)
                .Build();

            var mongoService = new MongoDbService(configuration);

            var controller = new InvitationController(mongoService);

            // Act

            var result = await controller.GenerateInvite("");

            // Assert

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);

            Assert.Equal("EventId is required", badRequestResult.Value);
        }

        [Fact]
        public async Task GenerateInvite_ValidEventId_ReturnsOk()
        {
            // Arrange

            var settings = new Dictionary<string, string>
            {
        { "ConnectionStrings:DbConnection", "mongodb://localhost/monodemo" }
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(settings!)
                .Build();

            var mongoService = new MongoDbService(configuration);

            var controller = new InvitationController(mongoService);

            // Act

            var result = await controller.GenerateInvite("event123");

            // Assert

            var okResult = Assert.IsType<OkObjectResult>(result);

            Assert.NotNull(okResult.Value);
        }
    }
}