using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Reqnroll;
using Xunit;
using System.Security.Claims;
using EversayApi.Controllers;
using EversayApi.Data;

namespace EversayApi.BDDTests.StepDefinitions
{
    [Binding]
    public class InvitationStepDefinitions
    {
        private IActionResult _result;
        private InvitationController _controller;

        private string _inviteCode = "";

        public InvitationStepDefinitions()
        {
            var settings = new Dictionary<string, string>
            {
                { "ConnectionStrings:DbConnection", "mongodb://localhost/monodemo" }
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(settings!)
                .Build();

            var mongoService = new MongoDbService(configuration);

            _controller = new InvitationController(mongoService);

            // Fake authenticated user

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "test-user")
            }, "TestAuthentication"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = user
                }
            };
        }

        [Given("a valid invitation code exists")]
        public void GivenAValidInvitationCodeExists()
        {
            _inviteCode = "17a739d9-9d8c-41b5-be90-74d66a6101c7";
        }

        [Given("an invalid invitation code")]
        public void GivenAnInvalidInvitationCode()
        {
            _inviteCode = "invalid-code";
        }

        [Given("an empty invitation code")]
        public void GivenAnEmptyInvitationCode()
        {
            _inviteCode = "";
        }

        [When("the user accepts the invitation")]
        public async Task WhenTheUserAcceptsTheInvitation()
        {
            _result = await _controller.AcceptInvite(_inviteCode);
        }

        [Then("the API should return {int} OK")]
        public void ThenTheAPIShouldReturnOK(int statusCode)
        {
            var okResult = Assert.IsType<OkObjectResult>(_result);

            Assert.Equal(statusCode, okResult.StatusCode ?? 200);
        }

        [Then("the API should return {int} NotFound")]
        public void ThenTheAPIShouldReturnNotFound(int statusCode)
        {
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(_result);

            Assert.Equal(statusCode, notFoundResult.StatusCode ?? 404);
        }

        [Then("the API should return {int} BadRequest")]
        public void ThenTheAPIShouldReturnBadRequest(int statusCode)
        {
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(_result);

            Assert.Equal(statusCode, badRequestResult.StatusCode ?? 400);
        }
    }
}