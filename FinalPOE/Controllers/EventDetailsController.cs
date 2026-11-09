using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;

using RaceDayAP1.Data;

using RaceDayAP1.Models;

namespace RaceDayApi.Controllers

{

    [ApiController]

    [Route("api/eventdetails")]

    public class EventDetailsController : ControllerBase

    {

        private readonly RaceDayContext _db;

        public EventDetailsController(RaceDayContext db)

        {

            _db = db;

        }

        // GET: api/eventdetails

        // Anyone can view event details.

        [HttpGet]

        public async Task<IActionResult> GetAll()

        {

            var details = await _db.EventDetails

                .AsNoTracking()

                .ToListAsync();

            return Ok(details);

        }

        // GET: api/eventdetails/{id}

        // Anyone can view details for a specific event.

        [HttpGet("{id}")]

        public async Task<IActionResult> GetById(string id)

        {

            var details = await _db.EventDetails

                .AsNoTracking()

                .FirstOrDefaultAsync(d => d.Event_ID == id);

            if (details == null)

            {

                return NotFound("Event details not found.");

            }

            return Ok(details);

        }

        // POST: api/eventdetails

        // Only Organisers can create event details.

        [Authorize(Roles = "Organiser")]

        [HttpPost]

        public async Task<IActionResult> Create(EventDetails details)

        {

            if (string.IsNullOrWhiteSpace(details.EventDetails_ID))

            {

                return BadRequest("Event Details ID is required.");

            }

            if (details.EventDetails_ID.Length > 13)

            {

                return BadRequest("Event Details ID cannot exceed 13 characters.");

            }

            if (string.IsNullOrWhiteSpace(details.Event_ID))

            {

                return BadRequest("Event ID is required.");

            }

            if (string.IsNullOrWhiteSpace(details.Event_Name))

            {

                return BadRequest("Event name is required.");

            }

            if (await _db.EventDetails.AnyAsync(

                d => d.Event_ID == details.Event_ID))

            {

                return Conflict("Details for this event already exist.");

            }

            var raceEvent = await _db.RaceEvents.FindAsync(details.Event_ID);

            if (raceEvent == null)

            {

                return NotFound("The event does not exist.");

            }

            _db.EventDetails.Add(details);

            await _db.SaveChangesAsync();

            return CreatedAtAction(

                nameof(GetById),

                new { id = details.Event_ID },

                details);

        }

        // PUT: api/eventdetails/{id}

        // Only Organisers can update event details.

        [Authorize(Roles = "Organiser")]

        [HttpPut("{id}")]

        public async Task<IActionResult> Update(

            string id,

            EventDetails updatedDetails)

        {

            var details = await _db.EventDetails

                .FirstOrDefaultAsync(d => d.Event_ID == id);

            if (details == null)

            {

                return NotFound("Event details not found.");

            }

            if (string.IsNullOrWhiteSpace(updatedDetails.Event_Name))

            {

                return BadRequest("Event name is required.");

            }

            details.Event_Name = updatedDetails.Event_Name;

            details.Event_Description = updatedDetails.Event_Description;

            details.Event_Distance = updatedDetails.Event_Distance;

            details.Event_Type = updatedDetails.Event_Type;

            await _db.SaveChangesAsync();

            return Ok(details);

        }

        // DELETE: api/eventdetails/{id}

        // Only Organisers can delete event details.

        [Authorize(Roles = "Organiser")]

        [HttpDelete("{id}")]

        public async Task<IActionResult> Delete(string id)

        {

            var details = await _db.EventDetails

                .FirstOrDefaultAsync(d => d.Event_ID == id);

            if (details == null)

            {

                return NotFound("Event details not found.");

            }

            _db.EventDetails.Remove(details);

            await _db.SaveChangesAsync();

            return NoContent();

        }

    }

}