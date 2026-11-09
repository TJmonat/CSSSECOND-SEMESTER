using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Authorization;

using RaceDayAP1.Data;

using RaceDayAP1.Models;

namespace RaceDayApi.Controllers

{

    [ApiController]

    [Route("api/raceresults")]

    public class RaceResultsController : ControllerBase

    {

        private readonly RaceDayContext _db;

        public RaceResultsController(RaceDayContext db)

        {

            _db = db;

        }

        // PARTICIPANT: View only their own race results

        [Authorize(Roles = "Participant")]

        [HttpGet("mine")]

        public async Task<IActionResult> GetMyResults()

        {

            var participantId = User.FindFirst("Participant_ID")?.Value;

            if (string.IsNullOrWhiteSpace(participantId))

                return Forbid();

            var results = await (

                from raceResult in _db.RaceResults.AsNoTracking()

                join registration in _db.Registrations.AsNoTracking()

                    on raceResult.Registration_ID

                    equals registration.Registration_ID

                join raceEvent in _db.RaceEvents.AsNoTracking()

                    on registration.EventID equals raceEvent.Event_ID

                join details in _db.EventDetails.AsNoTracking()

                    on raceEvent.Event_ID equals details.Event_ID

                    into eventDetails

                from details in eventDetails.DefaultIfEmpty()

                join selectedCategory in _db.RegistrationCategories.AsNoTracking()

                    on registration.Registration_ID

                    equals selectedCategory.Registration_ID

                    into registrationCategories

                from selectedCategory in registrationCategories.DefaultIfEmpty()

                join category in _db.EventCategories.AsNoTracking()

                    on selectedCategory.Category_ID

                    equals category.Category_ID

                    into categories

                from category in categories.DefaultIfEmpty()

                where registration.ParticipantID == participantId

                select new

                {

                    raceResult.RaceResult_ID,

                    registration.Registration_ID,

                    raceEvent.Event_ID,

                    Event_Name = details != null

                        ? details.Event_Name : "Event",

                    raceEvent.Event_Date,

                    raceEvent.Event_Location,

                    Category_ID = selectedCategory != null

                        ? selectedCategory.Category_ID : null,

                    Category_Name = category != null

                        ? category.Event_Category_Name : null,

                    raceResult.Finishing_Position,

                    raceResult.Finish_Time,

                    TotalParticipants = _db.Registrations.Count(r =>

                        r.EventID == registration.EventID)

                }

            ).ToListAsync();

            return Ok(results);

        }

        // ORGANISER: View all race results

        [Authorize(Roles = "Organiser")]

        [HttpGet]

        public async Task<IActionResult> GetAll()

        {

            var results = await _db.RaceResults

                .AsNoTracking()

                .ToListAsync();

            return Ok(results);

        }

        // ORGANISER: View one race result

        [Authorize(Roles = "Organiser")]

        [HttpGet("{id}")]

        public async Task<IActionResult> GetById(string id)

        {

            var result = await _db.RaceResults.FindAsync(id);

            if (result == null)

                return NotFound("Race result not found.");

            return Ok(result);

        }

        // ORGANISER: Add a race result

        [Authorize(Roles = "Organiser")]

        [HttpPost]

        public async Task<IActionResult> Create(RaceResult result)

        {

            if (string.IsNullOrWhiteSpace(result.RaceResult_ID))

                return BadRequest("Result ID is required.");

            if (result.RaceResult_ID.Length > 13)

                return BadRequest("Result ID cannot exceed 13 characters.");

            if (string.IsNullOrWhiteSpace(result.Registration_ID))

                return BadRequest("Registration ID is required.");

            if (result.Finishing_Position <= 0)

                return BadRequest("Finishing position must be greater than zero.");

            if (await _db.RaceResults.AnyAsync(r =>

                r.RaceResult_ID == result.RaceResult_ID))

                return Conflict("Result ID already exists.");

            if (await _db.RaceResults.AnyAsync(r =>

                r.Registration_ID == result.Registration_ID))

                return Conflict("This registration already has a result.");

            var registration = await _db.Registrations

                .FindAsync(result.Registration_ID);

            if (registration == null)

                return BadRequest("Registration does not exist.");

            _db.RaceResults.Add(result);

            await _db.SaveChangesAsync();

            return CreatedAtAction(

                nameof(GetById),

                new { id = result.RaceResult_ID },

                result);

        }

        // ORGANISER: Edit a race result

        [Authorize(Roles = "Organiser")]

        [HttpPut("{id}")]

        public async Task<IActionResult> Update(

            string id, RaceResult updatedResult)

        {

            var result = await _db.RaceResults.FindAsync(id);

            if (result == null)

                return NotFound("Race result not found.");

            if (updatedResult.Finishing_Position <= 0)

                return BadRequest("Finishing position must be greater than zero.");

            result.Finishing_Position = updatedResult.Finishing_Position;

            result.Finish_Time = updatedResult.Finish_Time;

            await _db.SaveChangesAsync();

            return Ok(result);

        }

        // ORGANISER: Delete a race result

        [Authorize(Roles = "Organiser")]

        [HttpDelete("{id}")]

        public async Task<IActionResult> Delete(string id)

        {

            var result = await _db.RaceResults.FindAsync(id);

            if (result == null)

                return NotFound("Race result not found.");

            _db.RaceResults.Remove(result);

            await _db.SaveChangesAsync();

            return NoContent();

        }

    }

}