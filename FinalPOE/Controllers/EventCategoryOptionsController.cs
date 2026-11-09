using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;

using RaceDayAP1.Data;

using RaceDayAP1.Models;

namespace RaceDayAP1.Controllers

{

    [Route("api/eventcategoryoptions")]

    [ApiController]

    public class EventCategoryOptionsController : ControllerBase

    {

        private readonly RaceDayContext _db;

        public EventCategoryOptionsController(RaceDayContext db)

        {

            _db = db;

        }

        // Get categories available for a particular event

        [HttpGet("{eventId}")]

        public async Task<IActionResult> GetCategories(string eventId)

        {

            var options = await _db.EventCategoryOptions

                .Where(x => x.Event_ID == eventId)

                .ToListAsync();

            return Ok(options);

        }

        // Organisers can assign a category to an event

        [HttpPost]

        [Authorize(Roles = "Organiser")]

        public async Task<IActionResult> AddCategory(

            EventCategoryOption option)

        {

            var eventExists = await _db.RaceEvents

                .AnyAsync(e => e.Event_ID == option.Event_ID);

            if (!eventExists)

            {

                return NotFound("Event not found.");

            }

            var categoryExists = await _db.EventCategories

                .AnyAsync(c => c.Category_ID == option.Category_ID);

            if (!categoryExists)

            {

                return NotFound("Category not found.");

            }

            var alreadyAdded = await _db.EventCategoryOptions

                .AnyAsync(x =>

                    x.Event_ID == option.Event_ID &&

                    x.Category_ID == option.Category_ID);

            if (alreadyAdded)

            {

                return Conflict("This category is already available for the event.");

            }

            _db.EventCategoryOptions.Add(option);

            await _db.SaveChangesAsync();

            return Ok("Category successfully assigned to event.");

        }

        // Organisers can remove a category from an event

        [HttpDelete]

        [Authorize(Roles = "Organiser")]

        public async Task<IActionResult> RemoveCategory(

            string eventId, string categoryId)

        {

            var option = await _db.EventCategoryOptions

                .FirstOrDefaultAsync(x =>

                    x.Event_ID == eventId &&

                    x.Category_ID == categoryId);

            if (option == null)

            {

                return NotFound("Category assignment not found.");

            }

            _db.EventCategoryOptions.Remove(option);

            await _db.SaveChangesAsync();

            return Ok("Category removed from event.");

        }

    }

}
