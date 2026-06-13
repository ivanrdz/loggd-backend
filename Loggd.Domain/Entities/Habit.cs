namespace Loggd.Domain.Entities;

public class Habit
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Color { get; set; } = "#6366f1";
    public string Emoji { get; set; } = "⭐";
    public HabitSchedule Schedule { get; set; } = HabitSchedule.Daily;
    public int CurrentStreak { get; set; } = 0;
    public int BestStreak { get; set; } = 0;
    public int TotalCompletions { get; set; } = 0;
    public bool IsArchived { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
    public ICollection<HabitLog> Logs { get; set; } = [];
}

public class HabitLog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid HabitId { get; set; }
    public DateOnly Date { get; set; }
    public int Count { get; set; } = 1;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Habit Habit { get; set; } = null!;
}

public enum HabitSchedule
{
    Daily,
    Weekdays,
    Weekends
}