

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;

using RaceDayAP1.Data;

using RaceDayAP1.Models;

namespace RaceDayApi.Controllers;

[ApiController]

[Route("api/participants")]
[Authorize(Roles = "Participant")]
public class ParticipantsController : ControllerBase
{
    private readonly RaceDayContext _db;
    public ParticipantsController(RaceDayContext db)
    {
        _db = db;
    }
   
    [HttpGet]

    public async Task<IActionResult> GetAll()
    {
        return Ok(await _db.Participants.AsNoTracking().ToListAsync());
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var participant = await _db.Participants.FindAsync(id);

        if (participant == null)

            return NotFound();

        return Ok(participant);

    }
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
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)

    {
        var participant = await _db.Participants.FindAsync(id);

        if (participant == null)

            return NotFound();

        bool hasRegistrations = await _db.Registrations

            .AnyAsync(r => r.ParticipantID == id);

        if (hasRegistrations)

            return Conflict("Remove the participant's registrations first.");

        _db.Participants.Remove(participant);

        await _db.SaveChangesAsync();

        return NoContent();

    }

}