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
public class EnrolmentsController : ControllerBase
{
    private readonly RaceDayDbContext _db;
    public EnrolmentsController(RaceDayDbContext db) => _db = db;

    private int CurrentUserId =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    // ---- Organiser: all enrolments across THEIR OWN events only ----
    [HttpGet]
    [Authorize(Roles = "Organiser")]
    public async Task<ActionResult<IEnumerable<Enrolment>>> GetForMyEvents() =>
        await _db.Enrolments
            .Include(e => e.Participant)
            .Include(e => e.Event)
            .Include(e => e.Category)
            .Include(e => e.Result)
            .Where(e => e.Event!.OrganiserId == CurrentUserId)
            .ToListAsync();

    // ---- Organiser: enrolments for one specific event they own ----
    [HttpGet("by-event/{eventId:int}")]
    [Authorize(Roles = "Organiser")]
    public async Task<ActionResult<IEnumerable<Enrolment>>> GetByEvent(int eventId)
    {
        var ev = await _db.Events.FindAsync(eventId);
        if (ev == null) return NotFound();
        if (ev.OrganiserId != CurrentUserId) return Forbid();

        return await _db.Enrolments
            .Include(e => e.Participant)
            .Include(e => e.Category)
            .Include(e => e.Result)
            .Where(e => e.EventId == eventId)
            .ToListAsync();
    }

    // ---- Participant: view own enrolments ----
    [HttpGet("my")]
    [Authorize(Roles = "Participant")]
    public async Task<ActionResult<IEnumerable<Enrolment>>> GetMine() =>
        await _db.Enrolments
            .Include(e => e.Event)
            .Include(e => e.Category)
            .Include(e => e.Result)
            .Where(e => e.ParticipantId == CurrentUserId)
            .ToListAsync();

    // ---- Get by id: participant sees own; organiser sees if it's in their event ----
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Enrolment>> GetById(int id)
    {
        var enr = await _db.Enrolments
            .Include(e => e.Participant)
            .Include(e => e.Event)
            .Include(e => e.Category)
            .Include(e => e.Result)
            .FirstOrDefaultAsync(e => e.EnrolmentId == id);
        if (enr == null) return NotFound();

        var isOwner = enr.ParticipantId == CurrentUserId;
        var isEventOrganiser = enr.Event!.OrganiserId == CurrentUserId;

        if (!isOwner && !isEventOrganiser) return Forbid();

        return enr;
    }

    // ---- Participant enrols (ParticipantId from token) ----
    [HttpPost]
    [Authorize(Roles = "Participant")]
    public async Task<ActionResult<Enrolment>> Create(Enrolment enrolment)
    {
        enrolment.ParticipantId = CurrentUserId;

        if (await _db.Enrolments.AnyAsync(e =>
                e.ParticipantId == enrolment.ParticipantId && e.EventId == enrolment.EventId))
            return Conflict("You are already enrolled in this event.");

        var category = await _db.Categories.FindAsync(enrolment.CategoryId);
        if (category == null) return BadRequest("Category does not exist.");
        if (category.EventId != enrolment.EventId)
            return BadRequest("Category does not belong to that event.");

        enrolment.Participant = null;
        enrolment.Event = null;
        enrolment.Category = null;
        enrolment.Result = null;
        enrolment.Status = "Pending";
        enrolment.EnrolmentDate = DateTime.UtcNow;

        _db.Enrolments.Add(enrolment);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = enrolment.EnrolmentId }, enrolment);
    }

    // ---- Organiser confirms enrolments in THEIR OWN events ----
    [HttpPatch("{id:int}/confirm")]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Confirm(int id)
    {
        var enr = await _db.Enrolments.Include(e => e.Event)
            .FirstOrDefaultAsync(e => e.EnrolmentId == id);
        if (enr == null) return NotFound();
        if (enr.Event!.OrganiserId != CurrentUserId) return Forbid();

        enr.Status = "Confirmed";
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // ---- Participant cancels own enrolment ----
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Participant")]
    public async Task<IActionResult> Delete(int id)
    {
        var enr = await _db.Enrolments.FindAsync(id);
        if (enr == null) return NotFound();
        if (enr.ParticipantId != CurrentUserId) return Forbid();

        _db.Enrolments.Remove(enr);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}