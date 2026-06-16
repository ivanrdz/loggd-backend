using Loggd.Domain.Entities;
using Loggd.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Loggd.Infrastructure.Services;

public class GoalService(AppDbContext db)
{
    public async Task<List<Goal>> GetUserGoalsAsync(Guid userId)
    {
        return await db.Goals
            .Where(g => g.UserId == userId && g.Status != GoalStatus.Abandoned)
            .OrderByDescending(g => g.CreatedAt)
            .ToListAsync();
    }

    public async Task<Goal> CreateGoalAsync(
        Guid userId, string title, string? description,
        string category, decimal? targetValue, string? unit, DateTime? targetDate)
    {
        var cat = Enum.Parse<GoalCategory>(category, ignoreCase: true);

        var goal = new Goal
        {
            UserId = userId,
            Title = title,
            Description = description,
            Category = cat,
            TargetValue = targetValue,
            CurrentValue = 0,
            Unit = unit,
            TargetDate = targetDate,
            Status = GoalStatus.Active
        };

        db.Goals.Add(goal);
        await db.SaveChangesAsync();
        return goal;
    }

    public async Task<Goal> LogProgressAsync(Guid userId, Guid goalId, decimal value, string? note)
    {
        var goal = await db.Goals
            .FirstOrDefaultAsync(g => g.Id == goalId && g.UserId == userId)
            ?? throw new KeyNotFoundException("Meta no encontrada.");

        // No permitir valores mayores al target
        if (goal.TargetValue.HasValue)
            value = Math.Min(value, goal.TargetValue.Value);

        goal.CurrentValue = value;

        if (goal.TargetValue.HasValue && value >= goal.TargetValue.Value)
            goal.Status = GoalStatus.Completed;

        await db.SaveChangesAsync();
        return goal;
    }

    public int GetProgressPercent(Goal goal)
    {
        if (!goal.TargetValue.HasValue || goal.TargetValue == 0) return 0;
        var percent = (goal.CurrentValue ?? 0) / goal.TargetValue.Value * 100;
        return Math.Min((int)percent, 100);
    }
}