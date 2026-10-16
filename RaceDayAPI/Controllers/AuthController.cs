using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Auth;
using RaceDayAPI.Data;
using RaceDayAPI.Models;

namespace RaceDayAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly RaceDayDbContext _db;
        private readonly JwtTokenService _jwt;

        public AuthController(RaceDayDbContext db, JwtTokenService jwt)
        {
            _db = db;
            _jwt = jwt;
        }

        // POST /api/auth/register
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register(RegisterRequest req)
        {
            // 1. Validate role — only Organiser or Participant allowed
            var allowedRoles = new[] { "Organiser", "Participant" };
            if (!allowedRoles.Contains(req.RoleName, StringComparer.OrdinalIgnoreCase))
                return BadRequest("Role must be either 'Organiser' or 'Participant'.");

            // 2. Check email uniqueness
            if (await _db.Users.AnyAsync(u => u.Email == req.Email))
                return Conflict("Email is already registered.");

            // 3. Look up role id
            var role = await _db.Roles
                .FirstOrDefaultAsync(r => r.RoleName == req.RoleName);
            if (role == null)
                return BadRequest("Invalid role.");

            // 4. Hash the password (never store plain)
            var user = new User
            {
                Email = req.Email,
                PasswordHash = PasswordHasher.Hash(req.Password),
                FirstName = req.FirstName,
                LastName = req.LastName,
                RoleId = role.RoleId,
                ProfilePictureUrl = req.ProfilePictureUrl,
                CreatedAt = DateTime.UtcNow
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            // 5. Issue a token immediately (auto-login after register)
            var token = _jwt.GenerateToken(user, role.RoleName);
            var expiry = DateTime.UtcNow.AddMinutes(60);

            return Ok(new AuthResponse
            {
                Token = token,
                ExpiresAt = expiry,
                UserId = user.UserId,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                RoleName = role.RoleName
            });
        }

        // POST /api/auth/login
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login(LoginRequest req)
        {
            var user = await _db.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == req.Email);

            // Generic error to avoid user enumeration
            if (user == null || !PasswordHasher.Verify(req.Password, user.PasswordHash))
                return Unauthorized("Invalid email or password.");

            var token = _jwt.GenerateToken(user, user.Role!.RoleName);
            var expiry = DateTime.UtcNow.AddMinutes(60);

            return Ok(new AuthResponse
            {
                Token = token,
                ExpiresAt = expiry,
                UserId = user.UserId,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                RoleName = user.Role.RoleName
            });
        }
    }
}
