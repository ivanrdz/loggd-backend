using Loggd.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Loggd.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AdminController(AppDbContext db) : ControllerBase
{
    private readonly string _adminEmail = "ivan.rodriguez.jaramillo@gmail.com";

    private bool IsAdmin => User.FindFirstValue(ClaimTypes.Email) == _adminEmail;

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        if (!IsAdmin) return Forbid();

        var users = await db.Users
            .OrderByDescending(u => u.CreatedAt)
            .Select(u => new
            {
                u.Id,
                u.Email,
                u.Name,
                u.AvatarUrl,
                u.Level,
                u.TotalXP,
                u.CreatedAt,
                u.LastActiveAt,
                HabitsCount = db.Habits.Count(h => h.UserId == u.Id),
                GoalsCount = db.Goals.Count(g => g.UserId == u.Id),
                TasksCount = db.Tasks.Count(t => t.UserId == u.Id)
            })
            .ToListAsync();

        return Ok(users);
    }
}