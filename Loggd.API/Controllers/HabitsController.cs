using Loggd.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Loggd.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class HabitsController(HabitService habitService) : ControllerBase
{
    private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await habitService.GetUserHabitsAsync(UserId));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateHabitRequest request)
    {
        var habit = await habitService.CreateHabitAsync(
            UserId, request.Name, request.Description, request.Color, request.Emoji);
        return Created("", habit);
    }

    [HttpPost("{id:guid}/checkin")]
    public async Task<IActionResult> CheckIn(Guid id)
    {
        try
        {
            var habit = await habitService.CheckInAsync(UserId, id);
            return Ok(habit);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("{id:guid}/contributions")]
    public async Task<IActionResult> GetContributions(Guid id, [FromQuery] int year = 0)
    {
        year = year == 0 ? DateTime.UtcNow.Year : year;
        var grid = await habitService.GetContributionsAsync(UserId, id, year);
        return Ok(grid);
    }
}

public record CreateHabitRequest(string Name, string? Description, string Color, string Emoji);