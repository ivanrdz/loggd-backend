using Loggd.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Loggd.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TasksController(TaskService taskService) : ControllerBase
{
    private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] bool includeCompleted = false)
        => Ok(await taskService.GetUserTasksAsync(UserId, includeCompleted));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTaskRequest request)
    {
        var task = await taskService.CreateTaskAsync(
            UserId,
            request.Title,
            request.Description,
            request.Priority,
            request.Tag,
            request.IsRecurring,
            request.DueDate
        );
        return Created("", task);
    }

    [HttpPost("{id:guid}/complete")]
    public async Task<IActionResult> Complete(Guid id)
    {
        try
        {
            var task = await taskService.CompleteTaskAsync(UserId, id);
            return Ok(task);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await taskService.DeleteTaskAsync(UserId, id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}

public record CreateTaskRequest(
    string Title,
    string? Description,
    string Priority,
    string? Tag,
    bool IsRecurring,
    DateTime? DueDate
);