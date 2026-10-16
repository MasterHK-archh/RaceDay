using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RaceDayAPI.Auth;
using RaceDayAPI.Data;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<RaceDayDbContext>(options =>
    options.UseInMemoryDatabase("RaceDayDB"));

// ---- JWT configuration ----
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()!;

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// ---- Swagger with Bearer auth ----
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token (without the 'Bearer ' prefix)."
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ---- Services ----
builder.Services.AddDbContext<RaceDayDbContext>(o => o.UseInMemoryDatabase("RaceDayDB"));
builder.Services.AddScoped<JwtTokenService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<RaceDayDbContext>();
    db.Database.EnsureCreated();

    // ---- Seed users with real BCrypt hashes (dev only) ----
    if (!db.Users.Any())
    {
        var organiserRole = db.Roles.First(r => r.RoleName == "Organiser");
        var participantRole = db.Roles.First(r => r.RoleName == "Participant");

        const string seedPassword = "Password123!";

        db.Users.AddRange(
            new RaceDayAPI.Models.User
            {
                UserId = 1,
                Email = "organiser_sipho@raceday.co.za",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(seedPassword),
                FirstName = "Sipho",
                LastName = "Khumalo",
                RoleId = organiserRole.RoleId,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new RaceDayAPI.Models.User
            {
                UserId = 2,
                Email = "organiser_chantal@raceday.co.za",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(seedPassword),
                FirstName = "Chantal",
                LastName = "Du Toit",
                RoleId = organiserRole.RoleId,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new RaceDayAPI.Models.User
            {
                UserId = 3,
                Email = "participant_jabu@raceday.co.za",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(seedPassword),
                FirstName = "Jabu",
                LastName = "Adele",
                RoleId = participantRole.RoleId,
                ProfilePictureUrl = "https://racedaystorage.blob.core.windows.net/profiles/jabu.jpg",
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new RaceDayAPI.Models.User
            {
                UserId = 4,
                Email = "participant_sarah@raceday.co.za",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(seedPassword),
                FirstName = "Sarah",
                LastName = "Smith",
                RoleId = participantRole.RoleId,
                ProfilePictureUrl = "https://racedaystorage.blob.core.windows.net/profiles/sarah.jpg",
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );

        db.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }