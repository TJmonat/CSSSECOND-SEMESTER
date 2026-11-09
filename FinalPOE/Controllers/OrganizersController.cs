using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;

using RaceDayAP1.Data;

using RaceDayAP1.Models;

namespace RaceDayApi.Controllers;

[ApiController]

[Route("api/organizers")]
[Authorize(Roles = "Organiser")]

public class OrganizersController : ControllerBase
{
    private readonly RaceDayContext _db;
    public OrganizersController(RaceDayContext db)
    {
        _db = db;
    }
    [HttpGet]

    public async Task<IActionResult> GetAll()
    {
        return Ok(await _db.EventOrganizers

            .AsNoTracking().ToListAsync());
    }

    [HttpGet("{id}")]

    public async Task<IActionResult> GetById(string id)
    {
        var organizer = await _db.EventOrganizers.FindAsync(id);

        return organizer == null ? NotFound() : Ok(organizer);
    }

    [HttpPost]

    public async Task<IActionResult> Create(EventOrganizer organizer)
    {
        if (string.IsNullOrWhiteSpace(organizer.Event_Organizer_ID))

            return BadRequest("Organizer ID is required.");

        if (organizer.Event_Organizer_ID.Length > 13)

            return BadRequest("Organizer ID cannot exceed 13 characters.");

        if (await _db.EventOrganizers.AnyAsync(

            o => o.Event_Organizer_ID == organizer.Event_Organizer_ID))

            return Conflict("Organizer ID already exists.");

        _db.EventOrganizers.Add(organizer);

        await _db.SaveChangesAsync();

        return CreatedAtAction(

            nameof(GetById),

            new { id = organizer.Event_Organizer_ID },

            organizer);

    }

    [HttpPut("{id}")]

    public async Task<IActionResult> Update(

        string id, EventOrganizer updated)

    {

        var organizer = await _db.EventOrganizers.FindAsync(id);

        if (organizer == null)

            return NotFound();

        organizer.Event_Organizer_Name = updated.Event_Organizer_Name;

        organizer.Event_Organizer_Surname = updated.Event_Organizer_Surname;

        organizer.Event_Organizers_Email = updated.Event_Organizers_Email;

        await _db.SaveChangesAsync();

        return Ok(organizer);

    }

    [HttpDelete("{id}")]

    public async Task<IActionResult> Delete(string id)
    {
        var organizer = await _db.EventOrganizers.FindAsync(id);

        if (organizer == null)

            return NotFound();
        if (await _db.RaceEvents.AnyAsync(
            e => e.EventOrganizerID == id))
            return Conflict("This organizer still manages events.");
        _db.EventOrganizers.Remove(organizer);
        await _db.SaveChangesAsync();
        return NoContent();

    }

}