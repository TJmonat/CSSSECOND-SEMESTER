using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;

using RaceDayAP1.Data;

using RaceDayAP1.Models;

namespace RaceDayApi.Controllers

{

    [ApiController]

    [Route("api/participants")]

    [Authorize(Roles = "Participant,Organiser")]

    public class ParticipantsController : ControllerBase

    {

        private readonly RaceDayContext _db;

        public ParticipantsController(RaceDayContext db)

        {

            _db = db;

        }

        // PARTICIPANT: VIEW OWN PROFILE

        [HttpGet("mine")]

        [Authorize(Roles = "Participant")]

        public async Task<IActionResult> GetMyProfile()

        {

            var participantId = User.FindFirst("Participant_ID")?.Value;

            if (string.IsNullOrWhiteSpace(participantId))

                return Forbid();

            var participant = await _db.Participants

                .AsNoTracking()

                .FirstOrDefaultAsync(p =>

                    p.Participant_ID == participantId);

            if (participant == null)

                return NotFound("Participant profile not found.");

            return Ok(participant);

        }

        // PARTICIPANT: UPDATE OWN PROFILE

        [HttpPut("mine")]

        [Authorize(Roles = "Participant")]

        public async Task<IActionResult> UpdateMyProfile(

            Participant updated)

        {

            var participantId = User.FindFirst("Participant_ID")?.Value;

            if (string.IsNullOrWhiteSpace(participantId))

                return Forbid();

            var participant = await _db.Participants

                .FirstOrDefaultAsync(p =>

                    p.Participant_ID == participantId);

            if (participant == null)

                return NotFound("Participant profile not found.");

            if (!string.IsNullOrWhiteSpace(updated.Participant_Email) &&

                await _db.Participants.AnyAsync(p =>

                    p.Participant_Email == updated.Participant_Email &&

                    p.Participant_ID != participantId))

            {

                return Conflict("Email already belongs to another participant.");

            }

            participant.Participant_Name = updated.Participant_Name;

            participant.Participant_Surname = updated.Participant_Surname;

            participant.Participant_Email = updated.Participant_Email;

            await _db.SaveChangesAsync();

            return Ok(participant);

        }

        // VIEW ALL PARTICIPANTS

        [HttpGet]

        public async Task<IActionResult> GetAll()

        {

            return Ok(await _db.Participants

                .AsNoTracking()

                .ToListAsync());

        }

        // VIEW PARTICIPANT BY ID

        [HttpGet("{id}")]

        public async Task<IActionResult> GetById(string id)

        {

            var participant = await _db.Participants.FindAsync(id);

            if (participant == null)

                return NotFound();

            return Ok(participant);

        }

        // CREATE PARTICIPANT

        [HttpPost]

        public async Task<IActionResult> Create(Participant participant)

        {

            if (string.IsNullOrWhiteSpace(participant.Participant_ID))

                return BadRequest("Participant ID is required.");

            if (participant.Participant_ID.Length > 13)

                return BadRequest("Participant ID cannot exceed 13 characters.");

            if (await _db.Participants.AnyAsync(

                p => p.Participant_ID == participant.Participant_ID))

                return Conflict("Participant ID already exists.");

            if (!string.IsNullOrWhiteSpace(participant.Participant_Email) &&

                await _db.Participants.AnyAsync(

                    p => p.Participant_Email == participant.Participant_Email))

                return Conflict("Email already exists.");

            _db.Participants.Add(participant);

            await _db.SaveChangesAsync();

            return CreatedAtAction(

                nameof(GetById),

                new { id = participant.Participant_ID },

                participant);

        }

        // UPDATE PARTICIPANT BY ID

        [HttpPut("{id}")]

        public async Task<IActionResult> Update(

            string id, Participant updated)

        {

            var participant = await _db.Participants.FindAsync(id);

            if (participant == null)

                return NotFound();

            if (!string.IsNullOrWhiteSpace(updated.Participant_Email) &&

                await _db.Participants.AnyAsync(

                    p => p.Participant_Email == updated.Participant_Email

                         && p.Participant_ID != id))

                return Conflict("Email already belongs to another participant.");

            participant.Participant_Name = updated.Participant_Name;

            participant.Participant_Surname = updated.Participant_Surname;

            participant.Participant_Email = updated.Participant_Email;

            await _db.SaveChangesAsync();

            return Ok(participant);

        }

        // DELETE PARTICIPANT

        [HttpDelete("{id}")]

        public async Task<IActionResult> Delete(string id)

        {

            var participant = await _db.Participants.FindAsync(id);

            if (participant == null)

                return NotFound();

            bool hasRegistrations = await _db.Registrations

                .AnyAsync(r => r.ParticipantID == id);

            if (hasRegistrations)

                return Conflict(

                    "Remove the participant's registrations first.");

            _db.Participants.Remove(participant);

            await _db.SaveChangesAsync();

            return NoContent();

        }

    }

}

