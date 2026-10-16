using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Data;
using RaceDayAPI.Models;
using System.Security.Claims;

namespace RaceDayAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EventsController : ControllerBase
{
    private readonly RaceDayDbContext _db;
    public EventsController(RaceDayDbContext db) => _db = db;

    private int CurrentUserId =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    // Both roles view all events
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<Event>>> GetAll() =>
        await _db.Events.Include(e => e.Organiser).Include(e => e.Categories).ToListAsync();

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<Event>> GetById(int id)
    {
        var ev = await _db.Events
            .Include(e => e.Organiser)
            .Include(e => e.Categories)
            .FirstOrDefaultAsync(e => e.EventId == id);
        return ev == null ? NotFound() : ev;
    }

    // Organiser's own events only
    [HttpGet("my")]
    [Authorize(Roles = "Organiser")]
    public async Task<ActionResult<IEnumerable<Event>>> GetMine() =>
        await _db.Events
            .Include(e => e.Categories)
            .Where(e => e.OrganiserId == CurrentUserId)
            .ToListAsync();

    [HttpPost]
    [Authorize(Roles = "Organiser")]
    public async Task<ActionResult<Event>> Create(Event ev)
    {
        if (!new[] { "Run", "Walk", "Cycle" }.Contains(ev.EventType, StringComparer.OrdinalIgnoreCase))
            return BadRequest("EventType must be Run, Walk, or Cycle.");
        if (ev.Distance <= 0)
            return BadRequest("Distance must be greater than 0.");

        // Force ownership from token — client cannot spoof OrganiserId
        ev.OrganiserId = CurrentUserId;
        ev.Organiser = null;
        ev.Categories.Clear();

        _db.Events.Add(ev);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = ev.EventId }, ev);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Update(int id, Event updated)
    {
        var ev = await _db.Events.FindAsync(id);
        if (ev == null) return NotFound();

        // Ownership check
        if (ev.OrganiserId != CurrentUserId) return Forbid();

        if (!new[] { "Run", "Walk", "Cycle" }.Contains(updated.EventType, StringComparer.OrdinalIgnoreCase))
            return BadRequest("EventType must be Run, Walk, or Cycle.");
        if (updated.Distance <= 0)
            return BadRequest("Distance must be greater than 0.");

        ev.Name = updated.Name;
        ev.Description = updated.Description;
        ev.Date = updated.Date;
        ev.Location = updated.Location;
        ev.Distance = updated.Distance;
        ev.EventType = updated.EventType;
        ev.BannerImageUrl = updated.BannerImageUrl;
        // OrganiserId never changes via update

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Delete(int id)
    {
        var ev = await _db.Events.FindAsync(id);
        if (ev == null) return NotFound();
        if (ev.OrganiserId != CurrentUserId) return Forbid();

        _db.Events.Remove(ev);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}