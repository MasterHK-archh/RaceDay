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
public class ResultsController : ControllerBase
{
    private readonly RaceDayDbContext _db;
    public ResultsController(RaceDayDbContext db) => _db = db;

    private int CurrentUserId =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    // ---- Participant: own results only ----
    [HttpGet("my")]
    [Authorize(Roles = "Participant")]
    public async Task<ActionResult<IEnumerable<object>>> GetMyResults()
    {
        return await _db.Results
            .Include(r => r.Enrolment)!.ThenInclude(e => e!.Event)
            .Include(r => r.Enrolment)!.ThenInclude(e => e!.Category)
            .Where(r => r.Enrolment!.ParticipantId == CurrentUserId)
            .Select(r => new
            {
                r.ResultId,
                r.FinishTime,
                r.FinishPosition,
                EventId = r.Enrolment!.EventId,
                EventName = r.Enrolment.Event!.Name,
                CategoryName = r.Enrolment.Category!.Name
            })
            .ToListAsync();
    }

    // ---- Organiser: results across their own events ----
    [HttpGet]
    [Authorize(Roles = "Organiser")]
    public async Task<ActionResult<IEnumerable<Result>>> GetForMyEvents() =>
        await _db.Results
            .Include(r => r.Enrolment)!.ThenInclude(e => e!.Event)
            .Where(r => r.Enrolment!.Event!.OrganiserId == CurrentUserId)
            .ToListAsync();

    // ---- Single result: participant owns it, or organiser owns the event ----
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Result>> GetById(int id)
    {
        var r = await _db.Results
            .Include(x => x.Enrolment)!.ThenInclude(e => e!.Event)
            .FirstOrDefaultAsync(x => x.ResultId == id);
        if (r == null) return NotFound();

        var isOwner = r.Enrolment!.ParticipantId == CurrentUserId;
        var isEventOrganiser = r.Enrolment.Event!.OrganiserId == CurrentUserId;
        if (!isOwner && !isEventOrganiser) return Forbid();

        return r;
    }

    // ---- Organiser captures results for THEIR events only ----
    [HttpPost]
    [Authorize(Roles = "Organiser")]
    public async Task<ActionResult<Result>> Create(Result result)
    {
        if (result.FinishPosition <= 0)
            return BadRequest("FinishPosition must be greater than 0.");

        var enrolment = await _db.Enrolments.Include(e => e.Event)
            .FirstOrDefaultAsync(e => e.EnrolmentId == result.EnrolmentId);
        if (enrolment == null) return BadRequest("Enrolment does not exist.");
        if (enrolment.Event!.OrganiserId != CurrentUserId) return Forbid();

        if (await _db.Results.AnyAsync(r => r.EnrolmentId == result.EnrolmentId))
            return Conflict("A result already exists for this enrolment.");

        result.Enrolment = null;
        _db.Results.Add(result);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = result.ResultId }, result);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Update(int id, Result updated)
    {
        var r = await _db.Results
            .Include(x => x.Enrolment)!.ThenInclude(e => e!.Event)
            .FirstOrDefaultAsync(x => x.ResultId == id);
        if (r == null) return NotFound();
        if (r.Enrolment!.Event!.OrganiserId != CurrentUserId) return Forbid();
        if (updated.FinishPosition <= 0)
            return BadRequest("FinishPosition must be greater than 0.");

        r.FinishTime = updated.FinishTime;
        r.FinishPosition = updated.FinishPosition;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Delete(int id)
    {
        var r = await _db.Results
            .Include(x => x.Enrolment)!.ThenInclude(e => e!.Event)
            .FirstOrDefaultAsync(x => x.ResultId == id);
        if (r == null) return NotFound();
        if (r.Enrolment!.Event!.OrganiserId != CurrentUserId) return Forbid();

        _db.Results.Remove(r);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}