using Loggd.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Loggd.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Habit> Habits => Set<Habit>();
    public DbSet<HabitLog> HabitLogs => Set<HabitLog>();
    public DbSet<Goal> Goals => Set<Goal>();
    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    protected override void OnModelCreating(ModelBuilder model)
    {
        model.Entity<User>(e =>
        {
            e.HasKey(u => u.Id);
            e.HasIndex(u => u.Email).IsUnique();
        });

        model.Entity<Habit>(e =>
        {
            e.HasKey(h => h.Id);
            e.HasOne(h => h.User)
             .WithMany(u => u.Habits)
             .HasForeignKey(h => h.UserId)
             .OnDelete(DeleteBehavior.Cascade);
            e.Property(h => h.Schedule).HasConversion<string>();
        });

        model.Entity<HabitLog>(e =>
        {
            e.HasKey(l => l.Id);
            e.HasIndex(l => new { l.HabitId, l.Date });
            e.HasOne(l => l.Habit)
             .WithMany(h => h.Logs)
             .HasForeignKey(l => l.HabitId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        model.Entity<Goal>(e =>
        {
            e.HasKey(g => g.Id);
            e.HasOne(g => g.User)
             .WithMany(u => u.Goals)
             .HasForeignKey(g => g.UserId)
             .OnDelete(DeleteBehavior.Cascade);
            e.Property(g => g.Category).HasConversion<string>();
            e.Property(g => g.Status).HasConversion<string>();
            e.Property(g => g.TargetValue).HasColumnType("decimal(18,2)");
            e.Property(g => g.CurrentValue).HasColumnType("decimal(18,2)");
        });

        model.Entity<TaskItem>(e =>
        {
            e.HasKey(t => t.Id);
            e.ToTable("Tasks");
            e.HasOne(t => t.User)
             .WithMany(u => u.Tasks)
             .HasForeignKey(t => t.UserId)
             .OnDelete(DeleteBehavior.Cascade);
            e.Property(t => t.Priority).HasConversion<string>();
        });
    }
}