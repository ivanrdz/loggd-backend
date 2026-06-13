using Loggd.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Loggd.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GoalsController(GoalService goalService) : ControllerBase
{
    private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await goalService.GetUserGoalsAsync(UserId));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateGoalRequest request)
    {
        var goal = await goalService.CreateGoalAsync(
            UserId,
            request.Title,
            request.Description,
            request.Category,
            request.TargetValue,
            request.Unit,
            request.TargetDate
        );
        return Created("", goal);
    }

    [HttpPost("{id:guid}/progress")]
    public async Task<IActionResult> LogProgress(Guid id, [FromBody] LogProgressRequest request)
    {
        try
        {
            var goal = await goalService.LogProgressAsync(UserId, id, request.Value, request.Note);
            return Ok(goal);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}

public record CreateGoalRequest(
    string Title,
    string? Description,
    string Category,
    decimal? TargetValue,
    string? Unit,
    DateTime? TargetDate
);

public record LogProgressRequest(decimal Value, string? Note);