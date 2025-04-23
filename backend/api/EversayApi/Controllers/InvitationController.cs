using Event_lib;
using EversayApi.Data;
using EversayApi.Models;
using GuestList_lib;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Security.Claims;

namespace EversayApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class InvitationController : ControllerBase
    {
        private readonly IMongoCollection<Invitation> _invitations;
        private readonly IMongoCollection<Event> _events;
        private readonly IMongoCollection<GuestList> _guestList;

        public InvitationController(MongoDbService mongoDbService)
        {
            _invitations = mongoDbService.Database.GetCollection<Invitation>("invitations");
            _events = mongoDbService.Database.GetCollection<Event>("event");
            _guestList = mongoDbService.Database.GetCollection<GuestList>("guestlists");
        }

        [HttpPost("event/{eventId}")]
        public async Task<IActionResult> GenerateInvite(string eventId)
        {
            var inviteCode = Guid.NewGuid().ToString();

            var invitation = new Invitation
            {
                InviteCode = inviteCode,
                EventId = eventId
            };

            await _invitations.InsertOneAsync(invitation);

            var request = HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";
            var link = $"{baseUrl}/invite/{inviteCode}";
            return Ok(new { inviteLink = link });
        }
        [HttpPost("accept/{inviteCode}")]
        public async Task<IActionResult> AcceptInvite(string inviteCode)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var invitation = await _invitations.Find(i => i.InviteCode == inviteCode).FirstOrDefaultAsync();
            if (invitation == null)
            {
                return NotFound("Invitation not found");
            }

            // Avoid duplicate guestlist
            var guestList = new GuestList
            {
                EventId = invitation.EventId,
                UserId = userId,
                IsAttending = true,
                GuestListName = "InviteJoin",
                Attendees = 1,
                GuestListImage = "",
                CreatedAt = DateTime.UtcNow
            };

            // Insert only if not already in guest list
            var existing = await _guestList.Find(x => x.EventId == invitation.EventId && x.UserId == userId).AnyAsync();
            if (!existing)
            {
                await _guestList.InsertOneAsync(guestList);

                var update = Builders<Invitation>.Update
                    .Set(x => x.AcceptedUserId, userId)
                    .Set(x => x.AcceptedAt, DateTime.UtcNow);

                await _invitations.UpdateOneAsync(i => i.InviteCode == inviteCode, update);
            }

            var ev = await _events.Find(e => e.eventId == invitation.EventId).FirstOrDefaultAsync();
            return Ok(ev);
        }


        [HttpPost("decline/{inviteCode}")]
        public async Task<IActionResult> DeclineInvite(string inviteCode)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var invitation = await _invitations.Find(i => i.InviteCode == inviteCode).FirstOrDefaultAsync();
            if (invitation == null)
            {
                return NotFound("Invitation not found");
            }

            
            var existing = await _guestList.Find(x => x.EventId == invitation.EventId && x.UserId == userId).FirstOrDefaultAsync();

            if (existing != null)
            {
                var update = Builders<GuestList>.Update
                    .Set(x => x.IsAttending, false);
                await _guestList.UpdateOneAsync(x => x.EventId == invitation.EventId && x.UserId == userId, update);
            }
            else
            {
                
                var guestList = new GuestList
                {
                    EventId = invitation.EventId,
                    UserId = userId,
                    IsAttending = false,
                    GuestListName = "InviteJoin",
                    Attendees = 1,
                    GuestListImage = "",
                    CreatedAt = DateTime.UtcNow
                };
                await _guestList.InsertOneAsync(guestList);
            }

            
            var invitationUpdate = Builders<Invitation>.Update
                .Set(x => x.DeclinedUserId, userId)
                .Set(x => x.DeclinedAt, DateTime.UtcNow);
            await _invitations.UpdateOneAsync(i => i.InviteCode == inviteCode, invitationUpdate);
            var ev = await _events.Find(e => e.eventId == invitation.EventId).FirstOrDefaultAsync();
            return Ok(ev);
        }
    }
}
