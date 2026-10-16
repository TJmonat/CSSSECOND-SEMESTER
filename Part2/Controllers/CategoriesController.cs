using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;

using RaceDayAP1.Data;

using RaceDayAP1.Models;

namespace RaceDayApi.Controllers;

[ApiController]

[Route("api/categories")]

public class CategoriesController : ControllerBase

{
    private readonly RaceDayContext _db;
    public CategoriesController(RaceDayContext db)
    {
        _db = db;
    }
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _db.EventCategories

            .AsNoTracking().ToListAsync());
    }

    [HttpGet("{id}")]

    public async Task<IActionResult> GetById(string id)
    {
        var category = await _db.EventCategories.FindAsync(id);

        return category == null ? NotFound() : Ok(category);
    }

    [HttpPost]

    public async Task<IActionResult> Create(EventCategory category)

    {
        if (string.IsNullOrWhiteSpace(category.Category_ID))

            return BadRequest("Category ID is required.");

        if (category.Category_ID.Length > 13)

            return BadRequest("Category ID cannot exceed 13 characters.");

        if (await _db.EventCategories.AnyAsync(

            c => c.Category_ID == category.Category_ID))

            return Conflict("Category ID already exists.");

        _db.EventCategories.Add(category);

        await _db.SaveChangesAsync();

        return CreatedAtAction(

            nameof(GetById),

            new { id = category.Category_ID },

            category);

    }

    [HttpPut("{id}")]

    public async Task<IActionResult> Update(

        string id, EventCategory updated)

    {

        var category = await _db.EventCategories.FindAsync(id);

        if (category == null)

            return NotFound();

        category.Event_Category_Type = updated.Event_Category_Type;

        category.Event_Category_Name = updated.Event_Category_Name;

        await _db.SaveChangesAsync();

        return Ok(category);

    }

    [HttpDelete("{id}")]

    public async Task<IActionResult> Delete(string id)

    {

        var category = await _db.EventCategories.FindAsync(id);

        if (category == null)

            return NotFound();

        if (await _db.RaceEvents.AnyAsync(

            e => e.CategoryID == id))

            return Conflict("The category is currently used by an event.");

        _db.EventCategories.Remove(category);

        await _db.SaveChangesAsync();

        return NoContent();

    }

}