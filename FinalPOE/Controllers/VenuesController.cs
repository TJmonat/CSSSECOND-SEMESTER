using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;

using RaceDayAP1.Data;

using RaceDayAP1.Models;

namespace RaceDayApi.Controllers;

[ApiController]

[Route("api/venues")]
public class VenuesController : ControllerBase
{
    private readonly RaceDayContext _db;
    public VenuesController(RaceDayContext db)
    {
        _db = db;
    }
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _db.Venues.AsNoTracking().ToListAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var venue = await _db.Venues.FindAsync(id);
        return venue == null ? NotFound() : Ok(venue);
    }
    [HttpPost]

    public async Task<IActionResult> Create(Venue venue)
    {
        if (string.IsNullOrWhiteSpace(venue.Venue_ID))

            return BadRequest("Venue ID is required.");

        if (venue.Venue_ID.Length > 13)

            return BadRequest("Venue ID cannot exceed 13 characters.");

        if (await _db.Venues.AnyAsync(v => v.Venue_ID == venue.Venue_ID))

            return Conflict("Venue ID already exists.");

        _db.Venues.Add(venue);

        await _db.SaveChangesAsync();

        return CreatedAtAction(

            nameof(GetById), new { id = venue.Venue_ID }, venue);

    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, Venue updated)
    {
        var venue = await _db.Venues.FindAsync(id);

        if (venue == null)

            return NotFound();

        venue.Venue_Name = updated.Venue_Name;

        venue.Venue_Location = updated.Venue_Location;

        await _db.SaveChangesAsync();

        return Ok(venue);
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var venue = await _db.Venues.FindAsync(id);

        if (venue == null)

            return NotFound();

        if (await _db.RaceEvents.AnyAsync(e => e.VenueID == id))

            return Conflict("This venue is used by an event.");

        _db.Venues.Remove(venue);

        await _db.SaveChangesAsync();

        return NoContent();

    }

}