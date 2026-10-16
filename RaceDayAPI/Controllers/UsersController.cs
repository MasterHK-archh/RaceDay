using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Auth;
using RaceDayAPI.Data;
using RaceDayAPI.Models;
using System.Security.Claims;

namespace RaceDayAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly RaceDayDbContext _db;
    public UsersController(RaceDayDbContext db) => _db = db;

    // Helper — current user's id from JWT
    private int CurrentUserId =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    // ---- GET /api/users/me — any authenticated user views own profile ----
    [HttpGet("me")]
    public async Task<ActionResult<object>> GetMyProfile()
    {
        var user = await _db.Users.Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.UserId == CurrentUserId);
        if (user == null) return NotFound();

        return new
        {
            user.UserId,
            user.Email,
            user.FirstName,
            user.LastName,
            RoleName = user.Role!.RoleName,
            user.ProfilePictureUrl,
            user.CreatedAt
        };
    }

    // ---- PUT /api/users/me — any authenticated user updates own profile ----
    [HttpPut("me")]
    public async Task<IActionResult> UpdateMyProfile(UpdateProfileRequest req)
    {
        var user = await _db.Users.FindAsync(CurrentUserId);
        if (user == null) return NotFound();

        // Email change → must remain unique
        if (!string.IsNullOrWhiteSpace(req.Email) && req.Email != user.Email)
        {
            if (await _db.Users.AnyAsync(u => u.Email == req.Email && u.UserId != user.UserId))
                return Conflict("Email already in use.");
            user.Email = req.Email;
        }

        if (!string.IsNullOrWhiteSpace(req.FirstName)) user.FirstName = req.FirstName;
        if (!string.IsNullOrWhiteSpace(req.LastName)) user.LastName = req.LastName;
        if (req.ProfilePictureUrl != null) user.ProfilePictureUrl = req.ProfilePictureUrl;

        // Optional password change
        if (!string.IsNullOrWhiteSpace(req.NewPassword))
        {
            if (string.IsNullOrWhiteSpace(req.CurrentPassword) ||
                !PasswordHasher.Verify(req.CurrentPassword, user.PasswordHash))
                return BadRequest("Current password is incorrect.");

            if (req.NewPassword.Length < 8)
                return BadRequest("New password must be at least 8 characters.");

            user.PasswordHash = PasswordHasher.Hash(req.NewPassword);
        }

        await _db.SaveChangesAsync();
        return NoContent();
    }

    // ---- Organisers can list users (unchanged) ----
    [HttpGet]
    [Authorize(Roles = "Organiser")]
    public async Task<ActionResult<IEnumerable<object>>> GetAll() =>
        await _db.Users.Include(u => u.Role)
            .Select(u => new
            {
                u.UserId,
                u.Email,
                u.FirstName,
                u.LastName,
                RoleName = u.Role!.RoleName,
                u.ProfilePictureUrl,
                u.CreatedAt
            }).ToListAsync();

    // ---- Any authenticated user can view a public profile ----
    [HttpGet("{id:int}")]
    public async Task<ActionResult<object>> GetById(int id)
    {
        var u = await _db.Users.Include(x => x.Role)
            .FirstOrDefaultAsync(x => x.UserId == id);
        if (u == null) return NotFound();

        return new
        {
            u.UserId,
            u.Email,
            u.FirstName,
            u.LastName,
            RoleName = u.Role!.RoleName,
            u.ProfilePictureUrl,
            u.CreatedAt
        };
    }
}