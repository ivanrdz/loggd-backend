using Loggd.Domain.Entities;
using Loggd.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Loggd.Infrastructure.Services;

public class TaskService(AppDbContext db)
{
    public async Task<List<TaskItem>> GetUserTasksAsync(Guid userId, bool includeCompleted = false)
    {
        return await db.Tasks
            .Where(t => t.UserId == userId && (includeCompleted || !t.IsCompleted))
            .OrderBy(t => t.IsCompleted)
            .ThenBy(t => t.DueDate)
            .ThenByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<TaskItem> CreateTaskAsync(
        Guid userId, string title, string? description,
        string priority, string? tag, bool isRecurring, DateTime? dueDate)
    {
        var p = Enum.Parse<TaskPriority>(priority, ignoreCase: true);

        var task = new TaskItem
        {
            UserId = userId,
            Title = title,
            Description = description,
            Priority = p,
            Tag = tag,
            IsRecurring = isRecurring,
            DueDate = dueDate
        };

        db.Tasks.Add(task);
        await db.SaveChangesAsync();
        return task;
    }

    public async Task<TaskItem> CompleteTaskAsync(Guid userId, Guid taskId)
    {
        var task = await db.Tasks
            .FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId)
            ?? throw new KeyNotFoundException("Tarea no encontrada.");

        task.IsCompleted = !task.IsCompleted;
        task.CompletedAt = task.IsCompleted ? DateTime.UtcNow : null;

        await db.SaveChangesAsync();
        return task;
    }

    public async Task DeleteTaskAsync(Guid userId, Guid taskId)
    {
        var task = await db.Tasks
            .FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId)
            ?? throw new KeyNotFoundException("Tarea no encontrada.");

        db.Tasks.Remove(task);
        await db.SaveChangesAsync();
    }
}