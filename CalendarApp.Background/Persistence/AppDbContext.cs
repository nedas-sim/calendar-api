using CalendarApp.Background.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace CalendarApp.Background.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options)
{
    public DbSet<WorkDayCheck> WorkDayChecks => Set<WorkDayCheck>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        
        base.OnModelCreating(modelBuilder);
    }
}
