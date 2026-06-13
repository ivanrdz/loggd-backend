namespace Loggd.Domain.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public string? GoogleId { get; set; }
    public int Level { get; set; } = 1;
    public int TotalXP { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastActiveAt { get; set; }

    public ICollection<Habit> Habits { get; set; } = [];
    public ICollection<Goal> Goals { get; set; } = [];
    public ICollection<TaskItem> Tasks { get; set; } = [];
}