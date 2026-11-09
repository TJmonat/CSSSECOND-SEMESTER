using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;

using RaceDayAP1.Data;

using RaceDayAP1.Models;

namespace RaceDayApi.Controllers

{

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

                .AsNoTracking()

                .ToListAsync());

        }
        [Authorize(Roles = "Participant")]

        [HttpGet("mine")]

        public async Task<IActionResult> GetMyEnrolments()

        {

            var participantId = User.FindFirst("Participant_ID")?.Value;

            if (string.IsNullOrWhiteSpace(participantId))

            {

                return Forbid();

            }

            var enrolments = await (

                from registration in _db.Registrations.AsNoTracking()

                join raceEvent in _db.RaceEvents.AsNoTracking()

                    on registration.EventID equals raceEvent.Event_ID

                join details in _db.EventDetails.AsNoTracking()

                    on raceEvent.Event_ID equals details.Event_ID

                    into eventDetails

                from details in eventDetails.DefaultIfEmpty()

                join selectedCategory in _db.RegistrationCategories.AsNoTracking()

                    on registration.Registration_ID equals selectedCategory.Registration_ID

                    into registrationCategories

                from selectedCategory in registrationCategories.DefaultIfEmpty()

                join category in _db.EventCategories.AsNoTracking()

                    on selectedCategory.Category_ID equals category.Category_ID

                    into categories

                from category in categories.DefaultIfEmpty()

                where registration.ParticipantID == participantId

                select new

                {

                    registration.Registration_ID,

                    Event_ID = raceEvent.Event_ID,

                    Event_Name = details != null ? details.Event_Name : "Event",

                    raceEvent.Event_Location,

                    raceEvent.Event_Date,

                    Category_ID = selectedCategory != null

                        ? selectedCategory.Category_ID : null,

                    Category_Name = category != null

                        ? category.Event_Category_Name : null,

                    registration.Registration_Date,

                    registration.Registration_Time,

                    registration.Registration_Status

                }

            ).ToListAsync();

            return Ok(enrolments);

        }

        [Authorize(Roles = "Participant")]

        [HttpPost]

        public async Task<IActionResult> Create(

            [FromBody] EnrolmentRequest request)

        {

            var participantId =

                User.FindFirst("Participant_ID")?.Value;

            if (string.IsNullOrWhiteSpace(participantId))

            {

                return Forbid();

            }

            if (string.IsNullOrWhiteSpace(request.Registration_ID))

            {

                return BadRequest("Registration ID is required.");

            }

            if (request.Registration_ID.Length > 13)

            {

                return BadRequest("Registration ID cannot exceed 13 characters.");

            }

            if (string.IsNullOrWhiteSpace(request.EventID))

            {

                return BadRequest("Event ID is required.");

            }

            if (string.IsNullOrWhiteSpace(request.Category_ID))

            {

                return BadRequest("Please select a category.");

            }

            if (await _db.Registrations.AnyAsync(r =>

                r.Registration_ID == request.Registration_ID))

            {

                return Conflict("Registration ID already exists.");

            }

            if (!await _db.Participants.AnyAsync(p =>

                p.Participant_ID == participantId))

            {

                return BadRequest("Participant does not exist.");

            }

            var selectedEvent = await _db.RaceEvents

                .FirstOrDefaultAsync(e =>

                    e.Event_ID == request.EventID);

            if (selectedEvent == null)

            {

                return BadRequest("Event does not exist.");

            }

            if (selectedEvent.Event_Date.Date < DateTime.Today)

            {

                return BadRequest(

                    "You cannot enrol for a completed event.");

            }

            if (await _db.Registrations.AnyAsync(r =>

                r.ParticipantID == participantId &&

                r.EventID == request.EventID))

            {

                return Conflict(

                    "You are already enrolled for this event.");

            }

            var categoryAvailable =

                await _db.EventCategoryOptions.AnyAsync(x =>

                    x.Event_ID == request.EventID &&

                    x.Category_ID == request.Category_ID);

            if (!categoryAvailable)

            {

                return BadRequest(

                    "The selected category is not available for this event.");

            }

            var registration = new Registration

            {

                Registration_ID = request.Registration_ID,

                EventID = request.EventID,

                ParticipantID = participantId,

                Registration_Status = "Pending",

                Registration_Date = DateTime.Today,

                Registration_Time = DateTime.Now.TimeOfDay

            };

            var registrationCategory = new RegistrationCategory

            {

                Registration_ID = request.Registration_ID,

                Category_ID = request.Category_ID

            };

            _db.Registrations.Add(registration);

            _db.RegistrationCategories.Add(registrationCategory);

            await _db.SaveChangesAsync();

            return StatusCode(201, new

            {

                Message = "Enrolment successful.",

                Registration = registration,

                SelectedCategory = registrationCategory

            });

        }

        [HttpDelete("{id}")]

        public async Task<IActionResult> Delete(string id)

        {

            var registration = await _db.Registrations.FindAsync(id);

            if (registration == null)

            {

                return NotFound();

            }

            _db.Registrations.Remove(registration);

            await _db.SaveChangesAsync();

            return NoContent();

        }

    }

    public class EnrolmentRequest

    {

        public string Registration_ID { get; set; } = "";

        public string EventID { get; set; } = "";

        public string Category_ID { get; set; } = "";

    }

}