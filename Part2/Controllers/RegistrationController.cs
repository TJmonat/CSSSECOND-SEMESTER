using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;

using RaceDayAP1.Data;

using RaceDayAP1.Models;

namespace RaceDayApi.Controllers;

[ApiController]

[Route("api/enrolments")]
public class RegistrationsController : ControllerBase
{
    private readonly RaceDayContext _db;
    public RegistrationsController(RaceDayContext db)
    {
        _db = db;
    }
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _db.Registrations

            .AsNoTracking().ToListAsync());

    }
    [HttpPost]
    public async Task<IActionResult> Create(Registration registration)
    {
        if (string.IsNullOrWhiteSpace(registration.Registration_ID))

            return BadRequest("Registration ID is required.");

        if (registration.Registration_ID.Length > 13)

            return BadRequest("Registration ID cannot exceed 13 characters.");

        if (string.IsNullOrWhiteSpace(registration.ParticipantID) ||

            string.IsNullOrWhiteSpace(registration.EventID))

            return BadRequest("Participant ID and Event ID are required.");

        if (await _db.Registrations.AnyAsync(

            r => r.Registration_ID == registration.Registration_ID))

            return Conflict("Registration ID already exists.");

        if (!await _db.Participants.AnyAsync(

            p => p.Participant_ID == registration.ParticipantID))

            return BadRequest("Participant does not exist.");

        if (!await _db.RaceEvents.AnyAsync(

            e => e.Event_ID == registration.EventID))

            return BadRequest("Event does not exist.");

        if (await _db.Registrations.AnyAsync(

            r => r.ParticipantID == registration.ParticipantID

                 && r.EventID == registration.EventID))

            return Conflict("Participant is already registered for this event.");

        registration.Registration_Status ??= "Pending";

        registration.Registration_Date ??= DateTime.Today;

        registration.Registration_Time ??= DateTime.Now.TimeOfDay;

        _db.Registrations.Add(registration);

        await _db.SaveChangesAsync();

        return StatusCode(201, registration);

    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var registration = await _db.Registrations.FindAsync(id);

        if (registration == null)

            return NotFound();

        _db.Registrations.Remove(registration);

        await _db.SaveChangesAsync();

        return NoContent();

    }

}
