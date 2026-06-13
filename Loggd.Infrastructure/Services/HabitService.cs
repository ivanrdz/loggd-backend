using Loggd.Domain.Entities;
using Loggd.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Loggd.Infrastructure.Services;

public class HabitService(AppDbContext db)
{
    public async Task<List<Habit>> GetUserHabitsAsync(Guid userId)
    {
        return await db.Habits
            .Where(h => h.UserId == userId && !h.IsArchived)
            .Include(h => h.Logs.OrderByDescending(l => l.Date).Take(30))
            .OrderByDescending(h => h.CreatedAt)
            .ToListAsync();
    }

    public async Task<Habit> CreateHabitAsync(Guid userId, string name, string? description, string color, string emoji)
    {
        var habit = new Habit
        {
            UserId = userId,
            Name = name,
            Description = description,
            Color = color,
            Emoji = emoji,
        };

        db.Habits.Add(habit);
        await db.SaveChangesAsync();
        return habit;
    }

    public async Task<Habit> CheckInAsync(Guid userId, Guid habitId)
    {
        var habit = await db.Habits
            .FirstOrDefaultAsync(h => h.Id == habitId && h.UserId == userId)
            ?? throw new KeyNotFoundException("Hábito no encontrado.");

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var logHoy = await db.HabitLogs
            .FirstOrDefaultAsync(l => l.HabitId == habitId && l.Date == today);

        if (logHoy is not null)
        {
            logHoy.Count++;
        }
        else
        {
            db.HabitLogs.Add(new HabitLog { HabitId = habitId, Date = today });
            habit.TotalCompletions++;
        }

        var todosLosLogs = await db.HabitLogs
            .Where(l => l.HabitId == habitId)
            .ToListAsync();

        habit.CurrentStreak = CalcularStreak(todosLosLogs);
        if (habit.CurrentStreak > habit.BestStreak)
            habit.BestStreak = habit.CurrentStreak;

        await db.SaveChangesAsync();
        return habit;
    }

    public async Task<List<object>> GetContributionsAsync(Guid userId, Guid habitId, int year)
    {
        return await db.HabitLogs
            .Where(l => l.HabitId == habitId
                     && l.Habit.UserId == userId
                     && l.Date.Year == year)
            .Select(l => new { date = l.Date, count = l.Count } as object)
            .ToListAsync();
    }

    private static int CalcularStreak(List<HabitLog> logs)
    {
        if (logs.Count == 0) return 0;

        var fechas = logs.Select(l => l.Date).OrderByDescending(d => d).ToList();
        var hoy = DateOnly.FromDateTime(DateTime.UtcNow);
        var streak = 0;
        var esperada = fechas.Contains(hoy) ? hoy : hoy.AddDays(-1);

        foreach (var fecha in fechas)
        {
            if (fecha == esperada)
            {
                streak++;
                esperada = esperada.AddDays(-1);
            }
            else if (fecha < esperada) break;
        }

        return streak;
    }
}