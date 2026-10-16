using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;

using RaceDayAP1.Data;

using RaceDayAP1.Models;

namespace RaceDayApi.Controllers;

[ApiController]

[Route("api/events")]

public class EventsController : ControllerBase

{
private readonly RaceDayContext _db;
public EventsController(RaceDayContext db)

{
 _db = db;
}
[HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var events = await _db.RaceEvents.AsNoTracking().ToListAsync();
        return Ok(events);
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)

    {
       var raceEvent = await _db.RaceEvents.FindAsync(id);

        if (raceEvent == null)

            return NotFound();

        return Ok(raceEvent);

    }


    [HttpPost]

    public async Task<IActionResult> Create(RaceEvent raceEvent)

    {

        if (string.IsNullOrWhiteSpace(raceEvent.Event_ID))

            return BadRequest("Event ID is required.");

        if (raceEvent.Event_ID.Length > 13)

            return BadRequest("Event ID cannot exceed 13 characters.");

        if (await _db.RaceEvents.AnyAsync(

            e => e.Event_ID == raceEvent.Event_ID))

            return Conflict("Event ID already exists.");

        if (!await _db.EventCategories.AnyAsync(

            c => c.Category_ID == raceEvent.CategoryID))

            return BadRequest("The category does not exist.");

        if (!await _db.EventOrganizers.AnyAsync(

            o => o.Event_Organizer_ID == raceEvent.EventOrganizerID))

            return BadRequest("The organiser does not exist.");

        if (!await _db.Venues.AnyAsync(

            v => v.Venue_ID == raceEvent.VenueID))

            return BadRequest("The venue does not exist.");

        _db.RaceEvents.Add(raceEvent);

        await _db.SaveChangesAsync();

        return CreatedAtAction(

            nameof(GetById),

            new { id = raceEvent.Event_ID },

            raceEvent);

    }

    

    [HttpPut("{id}")]

    public async Task<IActionResult> Update(

        string id, RaceEvent updated)

    {

        var raceEvent = await _db.RaceEvents.FindAsync(id);

        if (raceEvent == null)

            return NotFound();

        if (!await _db.EventCategories.AnyAsync(

            c => c.Category_ID == updated.CategoryID))

            return BadRequest("The category does not exist.");

        if (!await _db.EventOrganizers.AnyAsync(

            o => o.Event_Organizer_ID == updated.EventOrganizerID))

            return BadRequest("The organiser does not exist.");

        if (!await _db.Venues.AnyAsync(

            v => v.Venue_ID == updated.VenueID))

            return BadRequest("The venue does not exist.");

        raceEvent.Event_Location = updated.Event_Location;

        raceEvent.Event_Date = updated.Event_Date;

        raceEvent.CategoryID = updated.CategoryID;

        raceEvent.EventOrganizerID = updated.EventOrganizerID;

        raceEvent.VenueID = updated.VenueID;

        await _db.SaveChangesAsync();

        return Ok(raceEvent);

    }

    
    [HttpDelete("{id}")]

    public async Task<IActionResult> Delete(string id)

    {

        var raceEvent = await _db.RaceEvents.FindAsync(id);

        if (raceEvent == null)

            return NotFound();

        bool hasRegistrations = await _db.Registrations

            .AnyAsync(r => r.EventID == id);

        if (hasRegistrations)

            return Conflict("Remove the event registrations first.");

        _db.RaceEvents.Remove(raceEvent);

        await _db.SaveChangesAsync();

        return NoContent();

    }

}